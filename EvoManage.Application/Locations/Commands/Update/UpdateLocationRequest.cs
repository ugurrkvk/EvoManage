namespace EvoManage.Application.Locations.Commands.Update;

public sealed record UpdateLocationRequest(
    string Code,
    string? GroupCode);