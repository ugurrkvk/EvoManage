using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.Application.Abstractions.Persistence.Repositories;

public interface IStockMovementRepository
{
    Task AddAsync(
        StockMovement movement,
        CancellationToken cancellationToken = default);

    Task<decimal> GetStockAsync(
        int productId,
        int warehouseId,
        int locationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StockMovement>> GetPagedAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        StockMovementType? movementType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        StockMovementType? movementType,
        CancellationToken cancellationToken = default);
}