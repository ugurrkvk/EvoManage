namespace EvoManage.Application.Locations.Queries.GetList;

public sealed record GetLocationListRequest(
    int PageNumber = 1,
    int PageSize = 20);