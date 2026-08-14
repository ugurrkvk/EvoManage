namespace EvoManage.Application.Inventory.Stock.Queries.GetList;

public sealed record GetStockListResponse(
    IReadOnlyCollection<GetStockListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);