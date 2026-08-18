using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class SourceWarehouseValidationHandler(IActiveWarehouseValidator activeWarehouseValidator) : IStockTransferValidationHandler
{
    public StockTransferValidationStep Step => StockTransferValidationStep.SourceWarehouse;

    public async Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        context.SourceWarehouse = await activeWarehouseValidator.ValidateAsync(context.SourceWarehouseId, cancellationToken);
    }
}