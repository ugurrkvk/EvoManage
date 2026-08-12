using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace EvoManage.Infrastructure.Persistence.Repositories;

public class WarehouseRepository(ApplicationDbContext context) : GenericRepository<Warehouse>(context), IWarehouseRepository
{
    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        Context.Warehouses.AnyAsync(
            warehouse => warehouse.Code == code,
            cancellationToken);

    public async Task<IReadOnlyCollection<Warehouse>> GetPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default) => await Context.Warehouses
        .AsNoTracking()
        .OrderBy(warehouse => warehouse.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        Context.Warehouses.CountAsync(cancellationToken);

    public Task<bool> ExistsByCodeExceptIdAsync(string code, int excludedId,
        CancellationToken cancellationToken = default) => Context.Warehouses.AnyAsync(
        warehouse =>
            warehouse.Code == code &&
            warehouse.Id != excludedId,
        cancellationToken);

}