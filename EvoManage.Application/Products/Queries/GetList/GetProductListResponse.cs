namespace EvoManage.Application.Products.Queries.GetList;

public sealed record GetProductListResponse(
    IReadOnlyCollection<GetProductListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);