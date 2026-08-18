namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer;

public interface IStockTransferValidationHandler
{
    StockTransferValidationStep Step { get; }
    Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default);
}