using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Handlers;

public sealed class LocationValidationHandler(IWarehouseLocationValidator warehouseLocationValidator) : IStockMovementValidationHandler
{
    public StockMovementValidationStep Step => StockMovementValidationStep.Location;

    public async Task ValidateAsync(StockMovementValidationContext context, CancellationToken cancellationToken = default)
    {
        if (context.LocationId is null) return;
        context.Location = await warehouseLocationValidator.ValidateAsync(context.LocationId.Value, context.WarehouseId, cancellationToken);
    }
}