using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace EvoManage.Infrastructure.Persistence.Repositories;

public sealed class LocationRepository(
    ApplicationDbContext context)
    : GenericRepository<Location>(context), ILocationRepository
{
    public Task<bool> ExistsByCodeAsync(
        int warehouseId,
        string code,
        CancellationToken cancellationToken = default)
        => Context.Locations.AnyAsync(
            location =>
                location.WarehouseId == warehouseId &&
                location.Code == code,
            cancellationToken);

    public Task<bool> ExistsByCodeExceptIdAsync(
        int warehouseId,
        string code,
        int excludedId,
        CancellationToken cancellationToken = default)
        => Context.Locations.AnyAsync(
            location =>
                location.WarehouseId == warehouseId &&
                location.Code == code &&
                location.Id != excludedId,
            cancellationToken);

    public async Task<IReadOnlyCollection<Location>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        => await Context.Locations
            .AsNoTracking()
            .OrderBy(location => location.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public Task<int> CountAsync(
        CancellationToken cancellationToken = default)
        => Context.Locations.CountAsync(cancellationToken);
}