using EvoManage.Domain.Products;

namespace EvoManage.Application.Abstractions.Persistence.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Product>> GetPagedAsync(
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