using EvoManage.Domain.Locations;

namespace EvoManage.Application.Abstractions.Persistence.Repositories;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<bool> ExistsByCodeAsync(
        int warehouseId,
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeExceptIdAsync(
        int warehouseId,
        string code,
        int excludedId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Location>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        CancellationToken cancellationToken = default);
}