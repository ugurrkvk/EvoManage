using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Inventory.Common.StockAllocation;
using EvoManage.Application.Inventory.StockMovements.Commands.Issue;
using EvoManage.Application.Inventory.StockMovements.Commands.Receipt;
using EvoManage.Application.Inventory.StockMovements.Commands.Transfer;
using EvoManage.Application.Inventory.StockMovements.Events;
using EvoManage.Application.Inventory.StockMovements.Validation.Common;
using EvoManage.Application.Inventory.StockMovements.Validation.Transfer;
using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.Application.Inventory.StockMovements.Commands;

public sealed class StockMovementCommandService(
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    StockAllocationStrategyResolver stockAllocationStrategyResolver,
    StockMovementCreatedEventDispatcher stockMovementCreatedEventDispatcher,
    StockMovementValidationPipeline stockMovementValidationPipeline,
    StockTransferValidationPipeline stockTransferValidationPipeline)
{
    public async Task<CreateStockReceiptResponse> CreateReceiptAsync(CreateStockReceiptRequest request, CancellationToken cancellationToken = default)
    {
        var context = new StockMovementValidationContext(request.ProductId, request.WarehouseId, request.LocationId, request.Quantity);
        await stockMovementValidationPipeline.ValidateAsync(context, cancellationToken);

        var movement = StockMovement.Create(request.ProductId, request.WarehouseId, request.LocationId, request.Quantity, StockMovementType.Receipt);
        await stockMovementRepository.AddAsync(movement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var @event = new StockMovementCreatedEvent(
            MovementId: movement.Id,
            ProductId: movement.ProductId,
            WarehouseId: movement.WarehouseId,
            LocationId: movement.LocationId,
            Quantity: movement.Quantity,
            MovementType: movement.MovementType);
        await stockMovementCreatedEventDispatcher.DispatchAsync(@event, cancellationToken);
        return new CreateStockReceiptResponse(movement.Id);
    }

    public async Task<CreateStockIssueResponse> CreateIssueAsync(CreateStockIssueRequest request, CancellationToken cancellationToken = default)
    {
        var context = new StockMovementValidationContext(request.ProductId, request.WarehouseId, request.LocationId, request.Quantity);
        await stockMovementValidationPipeline.ValidateAsync(context, cancellationToken);

        var strategy = stockAllocationStrategyResolver.Resolve(request.AllocationStrategy);
        var allocations = await strategy.AllocateAsync(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.Quantity,
            cancellationToken);

        var movements = new List<StockMovement>();

        foreach (var allocation in allocations)
        {
            var movement = StockMovement.Create(
                request.ProductId,
                allocation.WarehouseId,
                allocation.LocationId,
                allocation.Quantity,
                StockMovementType.Issue);

            await stockMovementRepository.AddAsync(movement, cancellationToken);
            movements.Add(movement);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var movement in movements)
        {
            var @event = new StockMovementCreatedEvent(
                MovementId: movement.Id,
                ProductId: movement.ProductId,
                WarehouseId: movement.WarehouseId,
                LocationId: movement.LocationId,
                Quantity: movement.Quantity,
                MovementType: movement.MovementType);

            await stockMovementCreatedEventDispatcher.DispatchAsync(
                @event,
                cancellationToken);
        }

        var responseMovements = movements
            .Select(movement => new CreateStockIssueMovementResponse(
                movement.Id,
                movement.LocationId,
                movement.Quantity))
            .ToArray();

        return new CreateStockIssueResponse(responseMovements);
    }

    public async Task<CreateStockTransferResponse> CreateTransferAsync(CreateStockTransferRequest request, CancellationToken cancellationToken = default)
    {
        var context = new StockTransferValidationContext(
            request.ProductId,
            request.SourceWarehouseId,
            request.SourceLocationId,
            request.TargetWarehouseId,
            request.TargetLocationId,
            request.Quantity);
        await stockTransferValidationPipeline.ValidateAsync(context, cancellationToken);


        var transferOut = StockMovement.Create(request.ProductId, request.SourceWarehouseId, request.SourceLocationId, request.Quantity, StockMovementType.TransferOut);
        var transferIn = StockMovement.Create(request.ProductId, request.TargetWarehouseId, request.TargetLocationId, request.Quantity, StockMovementType.TransferIn);
        await stockMovementRepository.AddAsync(transferOut, cancellationToken);
        await stockMovementRepository.AddAsync(transferIn, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var transferOutEvent = new StockMovementCreatedEvent(
            MovementId: transferOut.Id,
            ProductId: transferOut.ProductId,
            WarehouseId: transferOut.WarehouseId,
            LocationId: transferOut.LocationId,
            Quantity: transferOut.Quantity,
            MovementType: transferOut.MovementType);
        await stockMovementCreatedEventDispatcher.DispatchAsync(transferOutEvent, cancellationToken);
        var transferInEvent = new StockMovementCreatedEvent(
            MovementId: transferIn.Id,
            ProductId: transferIn.ProductId,
            WarehouseId: transferIn.WarehouseId,
            LocationId: transferIn.LocationId,
            Quantity: transferIn.Quantity,
            MovementType: transferIn.MovementType);
        await stockMovementCreatedEventDispatcher.DispatchAsync(transferInEvent, cancellationToken);
        return new CreateStockTransferResponse(transferOut.Id, transferIn.Id);
    }
}