namespace EvoManage.Application.Inventory.Common.StockAllocation;

public sealed class StockAllocationStrategyResolver(IEnumerable<IStockAllocationStrategy> strategies)
{
    public IStockAllocationStrategy Resolve(StockAllocationStrategyType type)
    {
        return strategies.FirstOrDefault(strategy => strategy.Type == type) ?? throw new InvalidOperationException($"Stock allocation strategy '{type}' is not registered.");
    }
}