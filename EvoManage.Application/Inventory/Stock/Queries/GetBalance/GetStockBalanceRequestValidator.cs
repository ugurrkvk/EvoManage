using FluentValidation;

namespace EvoManage.Application.Inventory.Stock.Queries.GetBalance;

public sealed class GetStockBalanceRequestValidator
    : AbstractValidator<GetStockBalanceRequest>
{
    public GetStockBalanceRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0);

        RuleFor(request => request.WarehouseId)
            .GreaterThan(0);

        RuleFor(request => request.LocationId)
            .GreaterThan(0);
    }
}