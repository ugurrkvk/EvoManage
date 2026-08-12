namespace EvoManage.Application.Locations.Commands.Create;

public sealed record CreateLocationRequest(
    int WarehouseId,
    string Code,
    string? GroupCode);