using FluentValidation;

namespace EvoManage.Application.Warehouses.Commands.Create;

public sealed class CreateWarehouseRequestValidator
    : AbstractValidator<CreateWarehouseRequest>
{
    public CreateWarehouseRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Address)
            .MaximumLength(500);

        RuleFor(request => request.Description)
            .MaximumLength(1000);
    }
}