namespace EvoManage.Application.Inventory.StockMovements.Validation.Common;

public sealed class StockMovementValidationPipeline(IEnumerable<IStockMovementValidationHandler> handlers)
{
    public async Task ValidateAsync(StockMovementValidationContext context, CancellationToken cancellationToken = default)
    {
        foreach (var handler in handlers.OrderBy(handler => handler.Step))
        {
            await handler.ValidateAsync(context, cancellationToken);
        }
    }
}