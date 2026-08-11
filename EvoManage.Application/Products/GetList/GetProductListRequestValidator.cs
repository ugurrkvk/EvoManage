using FluentValidation;

namespace EvoManage.Application.Products.GetList;

public sealed class GetProductListRequestValidator
    : AbstractValidator<GetProductListRequest>
{
    public GetProductListRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);
    }
}