using FluentValidation;

namespace EvoManage.Application.Inventory.Stock.Queries.GetList;

public sealed class GetStockListRequestValidator
    : AbstractValidator<GetStockListRequest>
{
    public GetStockListRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0)
            .When(request => request.ProductId.HasValue);

        RuleFor(request => request.WarehouseId)
            .GreaterThan(0)
            .When(request => request.WarehouseId.HasValue);

        RuleFor(request => request.LocationId)
            .GreaterThan(0)
            .When(request => request.LocationId.HasValue);

        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);
    }
}