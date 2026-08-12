using FluentValidation;

namespace EvoManage.Application.Locations.Queries.GetList;

public sealed class GetLocationListRequestValidator
    : AbstractValidator<GetLocationListRequest>
{
    public GetLocationListRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);
    }
}