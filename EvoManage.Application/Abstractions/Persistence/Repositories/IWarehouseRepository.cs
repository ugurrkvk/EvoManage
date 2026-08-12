using EvoManage.Domain.Warehouses;

namespace EvoManage.Application.Abstractions.Persistence.Repositories;

public interface IWarehouseRepository : IGenericRepository<Warehouse>
{
    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Warehouse>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeExceptIdAsync(
        string code,
        int excludedId,
        CancellationToken cancellationToken = default);
}