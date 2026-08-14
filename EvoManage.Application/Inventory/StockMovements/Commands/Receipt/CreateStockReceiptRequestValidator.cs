using FluentValidation;

namespace EvoManage.Application.Inventory.StockMovements.Commands.Receipt;

public sealed class CreateStockReceiptRequestValidator
    : AbstractValidator<CreateStockReceiptRequest>
{
    public CreateStockReceiptRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0);

        RuleFor(request => request.WarehouseId)
            .GreaterThan(0);

        RuleFor(request => request.LocationId)
            .GreaterThan(0);

        RuleFor(request => request.Quantity)
            .GreaterThan(0);
    }
}