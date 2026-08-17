using EvoManage.Application.Integrations.ERP.Stock;

namespace EvoManage.Application.Inventory.StockMovements.Events.Handlers;

public sealed class ErpStockMovementCreatedEventHandler(IErpStockIntegration erpStockIntegration): IStockMovementCreatedEventHandler
{
    public async Task HandleAsync(StockMovementCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var movement = new ErpStockMovementModel(
            ProductId: @event.ProductId,
            WarehouseId: @event.WarehouseId,
            LocationId: @event.LocationId,
            Quantity: @event.Quantity,
            MovementType: @event.MovementType);
        await erpStockIntegration.SendStockMovementAsync(movement, cancellationToken);
    }
}