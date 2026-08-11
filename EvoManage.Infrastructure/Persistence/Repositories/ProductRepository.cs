using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace EvoManage.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        await Context.Products.AnyAsync(
            product => product.Code == code,
            cancellationToken);

    public async Task<IReadOnlyCollection<Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return context.Products.CountAsync(cancellationToken);
    }

    public Task<bool> ExistsByCodeExceptIdAsync(
        string code,
        int excludedId,
        CancellationToken cancellationToken = default)
    {
        return context.Products.AnyAsync(
            product => product.Code == code &&
                       product.Id != excludedId,
            cancellationToken);
    }
}