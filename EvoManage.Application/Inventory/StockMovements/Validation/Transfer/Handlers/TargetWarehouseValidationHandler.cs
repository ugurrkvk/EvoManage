using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class TargetWarehouseValidationHandler(IActiveWarehouseValidator activeWarehouseValidator) : IStockTransferValidationHandler
{
    public StockTransferValidationStep Step => StockTransferValidationStep.TargetWarehouse;

    public async Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        context.TargetWarehouse = await activeWarehouseValidator.ValidateAsync(context.TargetWarehouseId, cancellationToken);
    }
}