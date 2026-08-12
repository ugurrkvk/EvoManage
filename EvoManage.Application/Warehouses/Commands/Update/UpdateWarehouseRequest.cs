namespace EvoManage.Application.Warehouses.Commands.Update;

public sealed record UpdateWarehouseRequest(
    string Code,
    string Name,
    string? Address,
    string? Description);