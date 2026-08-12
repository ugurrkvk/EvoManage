namespace EvoManage.Application.Warehouses.Queries.GetList;

public sealed record GetWarehouseListItemResponse(
    int Id,
    string Code,
    string Name,
    string? Address,
    bool IsActive);