using FluentValidation;

namespace EvoManage.Application.Products.Commands.Update;

public sealed class UpdateProductRequestValidator
    : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.TrackingType)
            .IsInEnum();
    }
}