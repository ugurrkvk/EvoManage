using EvoManage.Application.Inventory.Stocks.Models;

namespace EvoManage.Application.Abstractions.Persistence.Repositories;

public interface IStockReadRepository
{
    Task<IReadOnlyCollection<StockBalanceModel>> GetPagedAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        bool includeZeroStock,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        bool includeZeroStock,
        CancellationToken cancellationToken = default);

    Task<decimal> GetBalanceAsync(
        int productId,
        int warehouseId,
        int locationId,
        CancellationToken cancellationToken = default);
}