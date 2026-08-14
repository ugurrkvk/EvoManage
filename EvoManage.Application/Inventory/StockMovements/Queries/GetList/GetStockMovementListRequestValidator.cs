using FluentValidation;

namespace EvoManage.Application.Inventory.StockMovements.Queries.GetList;

public sealed class GetStockMovementListRequestValidator
    : AbstractValidator<GetStockMovementListRequest>
{
    public GetStockMovementListRequestValidator()
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

        RuleFor(request => request.MovementType)
            .IsInEnum()
            .When(request => request.MovementType.HasValue);

        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);
    }
}