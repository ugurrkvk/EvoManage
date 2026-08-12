namespace EvoManage.Application.Locations.Queries.GetList;

public sealed record GetLocationListItemResponse(
    int Id,
    int WarehouseId,
    string Code,
    string? GroupCode,
    bool IsActive);