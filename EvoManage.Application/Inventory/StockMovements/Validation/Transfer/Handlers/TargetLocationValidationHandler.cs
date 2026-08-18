using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class TargetLocationValidationHandler(IWarehouseLocationValidator warehouseLocationValidator) : IStockTransferValidationHandler
{
    public StockTransferValidationStep Step => StockTransferValidationStep.TargetLocation;

    public async Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        context.TargetLocation = await warehouseLocationValidator.ValidateAsync(context.TargetLocationId, context.TargetWarehouseId, cancellationToken);
    }
}