namespace EvoManage.Application.Warehouses.Commands.Create;

public sealed record CreateWarehouseRequest(
    string Code,
    string Name,
    string? Address,
    string? Description);