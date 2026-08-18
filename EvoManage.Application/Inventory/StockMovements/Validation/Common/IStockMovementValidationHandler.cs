namespace EvoManage.Application.Inventory.StockMovements.Validation.Common;

public interface IStockMovementValidationHandler
{
    StockMovementValidationStep Step { get; }
    Task ValidateAsync(StockMovementValidationContext context, CancellationToken cancellationToken = default);
}