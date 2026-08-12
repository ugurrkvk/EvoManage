using FluentValidation;

namespace EvoManage.Application.Warehouses.Queries.GetList;

public sealed class GetWarehouseListRequestValidator
    : AbstractValidator<GetWarehouseListRequest>
{
    public GetWarehouseListRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);
    }
}