using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Inventory.Stocks.Models;

namespace EvoManage.Application.Inventory.Common.StockAllocation.Strategies;

public sealed class LowestStockAllocationStrategy(IStockReadRepository stockReadRepository) : OrderedStockAllocationStrategy(stockReadRepository)
{
    public override StockAllocationStrategyType Type => StockAllocationStrategyType.LowestStock;

    protected override IReadOnlyCollection<StockBalanceModel> OrderStocks(IReadOnlyCollection<StockBalanceModel> stocks)
    {
        return stocks
            .OrderBy(stock => stock.Quantity)
            .ThenBy(stock => stock.LocationId)
            .ToArray();
    }
}