using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Inventory.Common.StockAllocation.Strategies;

public sealed class ManualLocationAllocationStrategy(IStockReadRepository stockReadRepository, ILocationRepository locationRepository) : IStockAllocationStrategy
{
    public StockAllocationStrategyType Type => StockAllocationStrategyType.ManualLocation;

    public async Task<IReadOnlyCollection<StockAllocation>> AllocateAsync(int productId, int warehouseId, int? requestedLocationId, decimal quantity, CancellationToken cancellationToken = default)
    {
        if (requestedLocationId is null) throw new ConflictException("Location is required for manual stock allocation.");

        var location = await locationRepository.GetByIdAsync(requestedLocationId.Value, cancellationToken);
        
        if (location is null) throw new NotFoundException($"Location with id '{requestedLocationId}' was not found.");
        if (!location.IsActive) throw new ConflictException($"Location with id '{requestedLocationId}' is inactive.");
        if (location.WarehouseId != warehouseId) throw new ConflictException($"Location with id '{requestedLocationId}' does not belong to warehouse '{warehouseId}'.");

        var currentStock = await stockReadRepository.GetBalanceAsync(
            productId,
            warehouseId,
            requestedLocationId.Value,
            cancellationToken);

        return currentStock < quantity ? throw new ConflictException($"Insufficient stock. Available quantity is '{currentStock}'.") : [new StockAllocation(warehouseId, requestedLocationId.Value, quantity)];
    }
}