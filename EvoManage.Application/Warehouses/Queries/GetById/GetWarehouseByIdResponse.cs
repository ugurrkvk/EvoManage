namespace EvoManage.Application.Warehouses.Queries.GetById;

public sealed record GetWarehouseByIdResponse(
    int Id,
    string Code,
    string Name,
    string? Address,
    string? Description,
    bool IsActive);