using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Inventory.Common.StockAllocation.Strategies;

public sealed class HighestStockAllocationStrategy(IStockReadRepository stockReadRepository) : IStockAllocationStrategy
{
    public StockAllocationStrategyType Type => StockAllocationStrategyType.HighestStock;

    public async Task<IReadOnlyCollection<StockAllocation>> AllocateAsync(int productId, int warehouseId, int? requestedLocationId, decimal quantity, CancellationToken cancellationToken = default)
    {
        var availableStocks = await stockReadRepository.GetAvailableStocksAsync(productId, warehouseId, cancellationToken);

        var totalAvailableStock = availableStocks.Sum(stock => stock.Quantity);

        if (totalAvailableStock < quantity) throw new ConflictException($"Insufficient stock. Available quantity is '{totalAvailableStock}'.");

        var remainingQuantity = quantity;
        var allocations = new List<StockAllocation>();

        foreach (var stock in availableStocks)
        {
            if (remainingQuantity <= 0) break;
            var allocatedQuantity = Math.Min(stock.Quantity, remainingQuantity);
            allocations.Add(new StockAllocation(WarehouseId: stock.WarehouseId, LocationId: stock.LocationId, Quantity: allocatedQuantity));
            remainingQuantity -= allocatedQuantity;
        }

        return allocations;
    }
}