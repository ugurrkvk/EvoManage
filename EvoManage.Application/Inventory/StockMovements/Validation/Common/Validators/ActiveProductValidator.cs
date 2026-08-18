using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Domain.Products;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

public sealed class ActiveProductValidator(IProductRepository productRepository) : IActiveProductValidator
{
    public async Task<Product> ValidateAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null) throw new NotFoundException($"Product with id '{productId}' was not found.");
        if (!product.IsActive) throw new ConflictException($"Product with id '{productId}' is inactive.");
        return product;
    }
}