using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Handlers;

public sealed class WarehouseValidationHandler(IActiveWarehouseValidator activeWarehouseValidator) : IStockMovementValidationHandler
{
    public StockMovementValidationStep Step => StockMovementValidationStep.Warehouse;

    public async Task ValidateAsync(StockMovementValidationContext context, CancellationToken cancellationToken = default)
    {
        context.Warehouse = await activeWarehouseValidator.ValidateAsync(context.WarehouseId, cancellationToken);
    }
}