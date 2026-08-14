using FluentValidation;

namespace EvoManage.Application.Inventory.StockMovements.Commands.Transfer;

public sealed class CreateStockTransferRequestValidator
    : AbstractValidator<CreateStockTransferRequest>
{
    public CreateStockTransferRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0);

        RuleFor(request => request.SourceWarehouseId)
            .GreaterThan(0);

        RuleFor(request => request.SourceLocationId)
            .GreaterThan(0);

        RuleFor(request => request.TargetWarehouseId)
            .GreaterThan(0);

        RuleFor(request => request.TargetLocationId)
            .GreaterThan(0);

        RuleFor(request => request.Quantity)
            .GreaterThan(0);
    }
}