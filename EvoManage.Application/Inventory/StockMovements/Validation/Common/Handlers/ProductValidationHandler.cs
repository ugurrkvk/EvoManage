using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Handlers;

public sealed class ProductValidationHandler(IActiveProductValidator activeProductValidator) : IStockMovementValidationHandler
{
    public StockMovementValidationStep Step => StockMovementValidationStep.Product;

    public async Task ValidateAsync(StockMovementValidationContext context, CancellationToken cancellationToken = default)
    {
        context.Product = await activeProductValidator.ValidateAsync(context.ProductId, cancellationToken);
    }
}