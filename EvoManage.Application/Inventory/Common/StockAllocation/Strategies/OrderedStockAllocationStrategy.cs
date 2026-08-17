using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.Stocks.Models;

namespace EvoManage.Application.Inventory.Common.StockAllocation.Strategies;

public abstract class OrderedStockAllocationStrategy(IStockReadRepository stockReadRepository) : IStockAllocationStrategy
{
    public abstract StockAllocationStrategyType Type { get; }

    public async Task<IReadOnlyCollection<StockAllocation>> AllocateAsync(
        int productId,
        int warehouseId,
        int? requestedLocationId,
        decimal quantity,
        CancellationToken cancellationToken = default)
    {
        var availableStocks =
            await stockReadRepository.GetAvailableStocksAsync(
                productId,
                warehouseId,
                cancellationToken);

        var orderedStocks = OrderStocks(
            availableStocks);

        var totalAvailableStock = orderedStocks.Sum(
            stock => stock.Quantity);

        if (totalAvailableStock < quantity)
            throw new ConflictException(
                $"Insufficient stock. Available quantity is '{totalAvailableStock}'.");

        var remainingQuantity = quantity;
        var allocations = new List<StockAllocation>();

        foreach (var stock in orderedStocks)
        {
            if (remainingQuantity <= 0)
                break;

            var allocatedQuantity = Math.Min(
                stock.Quantity,
                remainingQuantity);

            allocations.Add(
                new StockAllocation(
                    WarehouseId: stock.WarehouseId,
                    LocationId: stock.LocationId,
                    Quantity: allocatedQuantity));

            remainingQuantity -= allocatedQuantity;
        }

        return allocations;
    }

    protected abstract IReadOnlyCollection<StockBalanceModel> OrderStocks(
        IReadOnlyCollection<StockBalanceModel> stocks);
}