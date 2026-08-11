namespace EvoManage.Application.Products.GetList;

public sealed record GetProductListRequest(
    int PageNumber = 1,
    int PageSize = 20);