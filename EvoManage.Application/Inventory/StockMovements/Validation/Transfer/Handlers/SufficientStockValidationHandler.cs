using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class SufficientStockValidationHandler(IStockMovementRepository stockMovementRepository) : IStockTransferValidationHandler
{
    public StockTransferValidationStep Step => StockTransferValidationStep.SufficientStock;
    public async Task ValidateAsync(StockTransferValidationContext context, CancellationToken cancellationToken = default)
    {
        var availableStock = await stockMovementRepository.GetStockAsync(context.ProductId, context.SourceWarehouseId, context.SourceLocationId, cancellationToken);
        if (availableStock < context.Quantity)
        {
            throw new ConflictException($"Insufficient stock for product {context.ProductId} in location {context.SourceLocationId}. Available: {availableStock}, Requested: {context.Quantity}");
        }
    }
}