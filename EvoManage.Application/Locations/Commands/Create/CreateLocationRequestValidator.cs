using FluentValidation;

namespace EvoManage.Application.Locations.Commands.Create;

public sealed class CreateLocationRequestValidator
    : AbstractValidator<CreateLocationRequest>
{
    public CreateLocationRequestValidator()
    {
        RuleFor(request => request.WarehouseId)
            .GreaterThan(0);

        RuleFor(request => request.Code)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.GroupCode)
            .MaximumLength(50);
    }
}