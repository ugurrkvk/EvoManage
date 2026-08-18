using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class DifferentSourceAndTargetValidationHandler : IStockTransferValidationHandler
{
    public StockTransferValidationStep Step => StockTransferValidationStep.DifferentSourceTarget;

    public Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        return context.SourceWarehouseId == context.TargetWarehouseId &&
               context.SourceLocationId == context.TargetLocationId
            ? throw new ConflictException("Source and target locations cannot be the same.")
            : Task.CompletedTask;
    }
}