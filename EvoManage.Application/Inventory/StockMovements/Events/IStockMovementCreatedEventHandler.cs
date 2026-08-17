namespace EvoManage.Application.Inventory.StockMovements.Events;

public interface IStockMovementCreatedEventHandler
{
    Task HandleAsync(StockMovementCreatedEvent @event, CancellationToken cancellationToken = default);
}