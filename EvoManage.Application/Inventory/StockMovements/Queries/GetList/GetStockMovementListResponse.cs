namespace EvoManage.Application.Inventory.StockMovements.Queries.GetList;

public sealed record GetStockMovementListResponse(
    IReadOnlyCollection<GetStockMovementListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);