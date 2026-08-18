namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer;

public sealed class StockTransferValidationPipeline(IEnumerable<IStockTransferValidationHandler> handlers)
{
    public async Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        foreach (var handler in handlers.OrderBy(handler => handler.Step))
        {
            await handler.ValidateAsync(context, cancellationToken);
        }
    }
}