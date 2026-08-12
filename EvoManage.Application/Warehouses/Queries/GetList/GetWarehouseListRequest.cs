namespace EvoManage.Application.Warehouses.Queries.GetList;

public sealed record GetWarehouseListRequest(
    int PageNumber = 1,
    int PageSize = 20);