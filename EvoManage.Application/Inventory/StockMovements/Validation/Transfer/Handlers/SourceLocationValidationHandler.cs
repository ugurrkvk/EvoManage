using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class SourceLocationValidationHandler(IWarehouseLocationValidator warehouseLocationValidator) : IStockTransferValidationHandler
{
    public StockTransferValidationStep Step => StockTransferValidationStep.SourceLocation;

    public async Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        context.SourceLocation = await warehouseLocationValidator.ValidateAsync(context.SourceLocationId, context.SourceWarehouseId, cancellationToken);
    }
}