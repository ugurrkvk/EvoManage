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
}