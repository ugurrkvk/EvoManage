using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Domain.Inventory.StockMovements;
using Microsoft.EntityFrameworkCore;

namespace EvoManage.Infrastructure.Persistence.Repositories;

public sealed class StockMovementRepository(
    ApplicationDbContext context) : IStockMovementRepository
{
    public async Task AddAsync(
        StockMovement movement,
        CancellationToken cancellationToken = default)
        => await context.StockMovements.AddAsync(
            movement,
            cancellationToken);

    public async Task<decimal> GetStockAsync(
        int productId,
        int warehouseId,
        int locationId,
        CancellationToken cancellationToken = default)
    {
        return await context.StockMovements
            .Where(movement =>
                movement.ProductId == productId &&
                movement.WarehouseId == warehouseId &&
                movement.LocationId == locationId)
            .SumAsync(
                movement =>
                    movement.MovementType == StockMovementType.Receipt ||
                    movement.MovementType == StockMovementType.TransferIn
                        ? movement.Quantity
                        : movement.MovementType == StockMovementType.Issue ||
                          movement.MovementType == StockMovementType.TransferOut
                            ? -movement.Quantity
                            : 0m,
                cancellationToken);
    }

    private IQueryable<StockMovement> ApplyFilters(
        int? productId,
        int? warehouseId,
        int? locationId,
        StockMovementType? movementType)
    {
        var query = context.StockMovements
            .AsNoTracking()
            .AsQueryable();

        if (productId.HasValue)
            query = query.Where(movement =>
                movement.ProductId == productId.Value);

        if (warehouseId.HasValue)
            query = query.Where(movement =>
                movement.WarehouseId == warehouseId.Value);

        if (locationId.HasValue)
            query = query.Where(movement =>
                movement.LocationId == locationId.Value);

        if (movementType.HasValue)
            query = query.Where(movement =>
                movement.MovementType == movementType.Value);

        return query;
    }

    public async Task<IReadOnlyCollection<StockMovement>> GetPagedAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        StockMovementType? movementType,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(
                productId,
                warehouseId,
                locationId,
                movementType)
            .OrderByDescending(movement => movement.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        StockMovementType? movementType,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(
                productId,
                warehouseId,
                locationId,
                movementType)
            .CountAsync(cancellationToken);
    }
}