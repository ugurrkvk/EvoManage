using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class TransferProductValidationHandler(IActiveProductValidator activeProductValidator) : IStockTransferValidationHandler
{
    public StockTransferValidationStep Step => StockTransferValidationStep.Product;
    public async Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        context.Product = await activeProductValidator.ValidateAsync(context.ProductId, cancellationToken);
    }
}