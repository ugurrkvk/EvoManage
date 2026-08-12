using FluentValidation;

namespace EvoManage.Application.Locations.Commands.Update;

public sealed class UpdateLocationRequestValidator
    : AbstractValidator<UpdateLocationRequest>
{
    public UpdateLocationRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.GroupCode)
            .MaximumLength(50);
    }
}