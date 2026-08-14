using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Inventory.Stocks.Models;
using EvoManage.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace EvoManage.Infrastructure.Persistence.Repositories;

public sealed class StockReadRepository(
    ApplicationDbContext context)
    : IStockReadRepository
{
    public async Task<IReadOnlyCollection<StockBalanceModel>> GetPagedAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        bool includeZeroStock,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(
                productId,
                warehouseId,
                locationId,
                includeZeroStock)
            .OrderBy(stock => stock.ProductCode)
            .ThenBy(stock => stock.WarehouseCode)
            .ThenBy(stock => stock.LocationCode)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(stock => new StockBalanceModel(
                stock.ProductId,
                stock.ProductCode,
                stock.ProductName,
                stock.WarehouseId,
                stock.WarehouseCode,
                stock.WarehouseName,
                stock.LocationId,
                stock.LocationCode,
                stock.Quantity))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        int? productId,
        int? warehouseId,
        int? locationId,
        bool includeZeroStock,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(
                productId,
                warehouseId,
                locationId,
                includeZeroStock)
            .CountAsync(cancellationToken);
    }

    public async Task<decimal> GetBalanceAsync(
        int productId,
        int warehouseId,
        int locationId,
        CancellationToken cancellationToken = default)
    {
        return await context.StockBalances
            .AsNoTracking()
            .Where(stock =>
                stock.ProductId == productId &&
                stock.WarehouseId == warehouseId &&
                stock.LocationId == locationId)
            .Select(stock => stock.Quantity)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<StockBalance> ApplyFilters(
        int? productId,
        int? warehouseId,
        int? locationId,
        bool includeZeroStock)
    {
        var query = context.StockBalances
            .AsNoTracking();

        if (productId.HasValue)
            query = query.Where(stock =>
                stock.ProductId == productId.Value);

        if (warehouseId.HasValue)
            query = query.Where(stock =>
                stock.WarehouseId == warehouseId.Value);

        if (locationId.HasValue)
            query = query.Where(stock =>
                stock.LocationId == locationId.Value);

        if (!includeZeroStock)
            query = query.Where(stock =>
                stock.Quantity != 0);

        return query;
    }
}