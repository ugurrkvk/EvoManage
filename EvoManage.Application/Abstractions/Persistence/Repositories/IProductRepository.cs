using EvoManage.Domain.Products;

namespace EvoManage.Application.Abstractions.Persistence.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}