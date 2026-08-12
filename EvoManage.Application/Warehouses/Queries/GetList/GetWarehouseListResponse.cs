namespace EvoManage.Application.Warehouses.Queries.GetList;

public sealed record GetWarehouseListResponse(
    IReadOnlyCollection<GetWarehouseListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);