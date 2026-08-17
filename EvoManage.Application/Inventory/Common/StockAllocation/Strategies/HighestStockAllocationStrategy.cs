using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Inventory.Stocks.Models;

namespace EvoManage.Application.Inventory.Common.StockAllocation.Strategies;

public sealed class HighestStockAllocationStrategy(IStockReadRepository stockReadRepository) : OrderedStockAllocationStrategy(stockReadRepository)
{
    public override StockAllocationStrategyType Type => StockAllocationStrategyType.HighestStock;

    protected override IReadOnlyCollection<StockBalanceModel> OrderStocks(IReadOnlyCollection<StockBalanceModel> stocks)
    {
        return stocks
            .OrderByDescending(stock => stock.Quantity)
            .ThenBy(stock => stock.LocationId)
            .ToArray();
    }
}