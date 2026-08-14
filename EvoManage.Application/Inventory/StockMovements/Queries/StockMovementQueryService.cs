using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Inventory.StockMovements.Queries.GetList;

namespace EvoManage.Application.Inventory.StockMovements.Queries;

public sealed class StockMovementQueryService(
    IStockMovementRepository stockMovementRepository)
{
    public async Task<GetStockMovementListResponse> GetListAsync(
        GetStockMovementListRequest request,
        CancellationToken cancellationToken = default)
    {
        var movements = await stockMovementRepository.GetPagedAsync(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.MovementType,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalCount = await stockMovementRepository.CountAsync(
            request.ProductId,
            request.WarehouseId,
            request.LocationId,
            request.MovementType,
            cancellationToken);

        var items = movements
            .Select(movement => new GetStockMovementListItemResponse(
                movement.Id,
                movement.ProductId,
                movement.WarehouseId,
                movement.LocationId,
                movement.Quantity,
                movement.MovementType))
            .ToArray();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)request.PageSize);

        return new GetStockMovementListResponse(
            items,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages);
    }
}