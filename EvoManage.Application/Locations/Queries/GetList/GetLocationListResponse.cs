namespace EvoManage.Application.Locations.Queries.GetList;

public sealed record GetLocationListResponse(
    IReadOnlyCollection<GetLocationListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);