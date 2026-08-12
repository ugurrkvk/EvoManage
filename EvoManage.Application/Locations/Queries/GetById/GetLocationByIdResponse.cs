namespace EvoManage.Application.Locations.Queries.GetById;

public sealed record GetLocationByIdResponse(
    int Id,
    int WarehouseId,
    string Code,
    string? GroupCode,
    bool IsActive);