namespace EvoManage.Application.Inventory.StockMovements.Events;

public sealed class StockMovementCreatedEventDispatcher(IEnumerable<IStockMovementCreatedEventHandler> handlers)
{
    public async Task DispatchAsync(StockMovementCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event, cancellationToken);
        }
    }
}