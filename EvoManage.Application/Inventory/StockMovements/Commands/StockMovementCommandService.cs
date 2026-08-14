using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Commands.Issue;
using EvoManage.Application.Inventory.StockMovements.Commands.Receipt;
using EvoManage.Application.Inventory.StockMovements.Commands.Transfer;
using EvoManage.Domain.Inventory.StockMovements;
using EvoManage.Domain.Locations;
using EvoManage.Domain.Products;
using EvoManage.Domain.Warehouses;

namespace EvoManage.Application.Inventory.StockMovements.Commands;

public sealed class StockMovementCommandService(
    IStockMovementRepository stockMovementRepository,
    IProductRepository productRepository,
    IWarehouseRepository warehouseRepository,
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateStockReceiptResponse> CreateReceiptAsync(
        CreateStockReceiptRequest request,
        CancellationToken cancellationToken = default)
    {
        await GetActiveProductAsync(
            request.ProductId,
            cancellationToken);

        await GetActiveWarehouseAsync(
            request.WarehouseId,
            cancellationToken: cancellationToken);

        await GetActiveLocationAsync(
            request.LocationId,
            request.WarehouseId,
            cancellationToken: cancellationToken);

        var movement = StockMovement.Create(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.Quantity,
            StockMovementType.Receipt);

        await stockMovementRepository.AddAsync(
            movement,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateStockReceiptResponse(movement.Id);
    }

    public async Task<CreateStockIssueResponse> CreateIssueAsync(
        CreateStockIssueRequest request,
        CancellationToken cancellationToken = default)
    {
        await GetActiveProductAsync(
            request.ProductId,
            cancellationToken);

        await GetActiveWarehouseAsync(
            request.WarehouseId,
            cancellationToken: cancellationToken);

        await GetActiveLocationAsync(
            request.LocationId,
            request.WarehouseId,
            cancellationToken: cancellationToken);

        await EnsureSufficientStockAsync(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.Quantity,
            cancellationToken);

        var movement = StockMovement.Create(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.Quantity,
            StockMovementType.Issue);

        await stockMovementRepository.AddAsync(
            movement,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateStockIssueResponse(movement.Id);
    }

    public async Task<CreateStockTransferResponse> CreateTransferAsync(
        CreateStockTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        await GetActiveProductAsync(
            request.ProductId,
            cancellationToken);

        await GetActiveWarehouseAsync(
            request.SourceWarehouseId,
            "Source",
            cancellationToken);

        await GetActiveLocationAsync(
            request.SourceLocationId,
            request.SourceWarehouseId,
            "Source",
            cancellationToken);

        await GetActiveWarehouseAsync(
            request.TargetWarehouseId,
            "Target",
            cancellationToken);

        await GetActiveLocationAsync(
            request.TargetLocationId,
            request.TargetWarehouseId,
            "Target",
            cancellationToken);

        EnsureDifferentSourceAndTarget(request);

        await EnsureSufficientStockAsync(
            request.ProductId,
            request.SourceWarehouseId,
            request.SourceLocationId,
            request.Quantity,
            cancellationToken);

        var transferOut = StockMovement.Create(
            request.ProductId,
            request.SourceWarehouseId,
            request.SourceLocationId,
            request.Quantity,
            StockMovementType.TransferOut);

        var transferIn = StockMovement.Create(
            request.ProductId,
            request.TargetWarehouseId,
            request.TargetLocationId,
            request.Quantity,
            StockMovementType.TransferIn);

        await stockMovementRepository.AddAsync(
            transferOut,
            cancellationToken);

        await stockMovementRepository.AddAsync(
            transferIn,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateStockTransferResponse(
            transferOut.Id,
            transferIn.Id);
    }

    private async Task<Product> GetActiveProductAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(
            productId,
            cancellationToken);

        if (product is null)
            throw new NotFoundException(
                $"Product with id '{productId}' was not found.");

        if (!product.IsActive)
            throw new ConflictException(
                $"Product with id '{productId}' is inactive.");

        return product;
    }

    private async Task<Warehouse> GetActiveWarehouseAsync(
        int warehouseId,
        string? role = null,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(
            warehouseId,
            cancellationToken);

        var prefix = string.IsNullOrWhiteSpace(role)
            ? "Warehouse"
            : $"{role} warehouse";

        if (warehouse is null)
            throw new NotFoundException(
                $"{prefix} with id '{warehouseId}' was not found.");

        if (!warehouse.IsActive)
            throw new ConflictException(
                $"{prefix} with id '{warehouseId}' is inactive.");

        return warehouse;
    }

    private async Task<Location> GetActiveLocationAsync(
        int locationId,
        int warehouseId,
        string? role = null,
        CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(
            locationId,
            cancellationToken);

        var prefix = string.IsNullOrWhiteSpace(role)
            ? "Location"
            : $"{role} location";

        if (location is null)
            throw new NotFoundException(
                $"{prefix} with id '{locationId}' was not found.");

        if (!location.IsActive)
            throw new ConflictException(
                $"{prefix} with id '{locationId}' is inactive.");

        if (location.WarehouseId != warehouseId)
            throw new ConflictException(
                $"{prefix} with id '{locationId}' does not belong to warehouse '{warehouseId}'.");

        return location;
    }

    private async Task EnsureSufficientStockAsync(
        int productId,
        int warehouseId,
        int locationId,
        decimal requestedQuantity,
        CancellationToken cancellationToken)
    {
        var currentStock = await stockMovementRepository.GetStockAsync(
            productId,
            warehouseId,
            locationId,
            cancellationToken);

        if (currentStock < requestedQuantity)
            throw new ConflictException(
                $"Insufficient stock. Available quantity is '{currentStock}'.");
    }

    private static void EnsureDifferentSourceAndTarget(
        CreateStockTransferRequest request)
    {
        if (request.SourceWarehouseId == request.TargetWarehouseId &&
            request.SourceLocationId == request.TargetLocationId)
            throw new ConflictException(
                "Source and target locations cannot be the same.");
    }
}