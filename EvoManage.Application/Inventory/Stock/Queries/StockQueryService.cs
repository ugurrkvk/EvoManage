using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.Stock.Queries.GetBalance;
using EvoManage.Application.Inventory.Stock.Queries.GetList;

namespace EvoManage.Application.Inventory.Stock.Queries;

public sealed class StockQueryService(
    IStockReadRepository stockReadRepository,
    IProductRepository productRepository,
    IWarehouseRepository warehouseRepository,
    ILocationRepository locationRepository)
{
    public async Task<GetStockBalanceResponse> GetBalanceAsync(
        GetStockBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
            throw new NotFoundException(
                $"Product with id '{request.ProductId}' was not found.");

        var warehouse = await warehouseRepository.GetByIdAsync(
            request.WarehouseId,
            cancellationToken);

        if (warehouse is null)
            throw new NotFoundException(
                $"Warehouse with id '{request.WarehouseId}' was not found.");

        var location = await locationRepository.GetByIdAsync(
            request.LocationId,
            cancellationToken);

        if (location is null)
            throw new NotFoundException(
                $"Location with id '{request.LocationId}' was not found.");

        if (location.WarehouseId != request.WarehouseId)
            throw new ConflictException(
                $"Location with id '{request.LocationId}' does not belong to warehouse '{request.WarehouseId}'.");

        var quantity = await stockReadRepository.GetBalanceAsync(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            cancellationToken);

        return new GetStockBalanceResponse(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            quantity);
    }

    public async Task<GetStockListResponse> GetListAsync(
        GetStockListRequest request,
        CancellationToken cancellationToken = default)
    {
        var stocks = await stockReadRepository.GetPagedAsync(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.IncludeZeroStock,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalCount = await stockReadRepository.CountAsync(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.IncludeZeroStock,
            cancellationToken);

        var items = stocks
            .Select(stock => new GetStockListItemResponse(
                stock.ProductId,
                stock.ProductCode,
                stock.ProductName,
                stock.WarehouseId,
                stock.WarehouseCode,
                stock.WarehouseName,
                stock.LocationId,
                stock.LocationCode,
                stock.Quantity))
            .ToArray();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)request.PageSize);

        return new GetStockListResponse(
            items,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages);
    }
}