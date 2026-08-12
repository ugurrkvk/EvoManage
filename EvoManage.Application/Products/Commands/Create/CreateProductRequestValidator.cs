using FluentValidation;

namespace EvoManage.Application.Products.Commands.Create;

public sealed class CreateProductRequestValidator
    : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
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