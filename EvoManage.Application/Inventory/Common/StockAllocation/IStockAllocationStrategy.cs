namespace EvoManage.Application.Inventory.Common.StockAllocation;

public interface IStockAllocationStrategy
{
    StockAllocationStrategyType Type { get; }

    Task<IReadOnlyCollection<StockAllocation>> AllocateAsync(int productId, int warehouseId, int? requestedLocationId, decimal quantity, CancellationToken cancellationToken = default);
}