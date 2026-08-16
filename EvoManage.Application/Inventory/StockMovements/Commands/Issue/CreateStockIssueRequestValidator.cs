using EvoManage.Application.Inventory.Common.StockAllocation;
using FluentValidation;

namespace EvoManage.Application.Inventory.StockMovements.Commands.Issue;

public sealed class CreateStockIssueRequestValidator : AbstractValidator<CreateStockIssueRequest>
{
    public CreateStockIssueRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0);

        RuleFor(request => request.WarehouseId)
            .GreaterThan(0);

        RuleFor(request => request.LocationId)
            .NotNull()
            .GreaterThan(0)
            .When(request => request.AllocationStrategy == StockAllocationStrategyType.ManualLocation);

        RuleFor(request => request.Quantity)
            .GreaterThan(0);

        RuleFor(request => request.AllocationStrategy)
            .IsInEnum();
    }
}