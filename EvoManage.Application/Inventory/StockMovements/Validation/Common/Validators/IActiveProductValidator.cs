using EvoManage.Domain.Products;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

public interface IActiveProductValidator
{
    Task<Product> ValidateAsync(int productId, CancellationToken cancellationToken = default);
}