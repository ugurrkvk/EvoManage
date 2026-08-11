namespace EvoManage.Application.Products.GetList;

public sealed record GetProductListResponse(
    IReadOnlyCollection<GetProductListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);