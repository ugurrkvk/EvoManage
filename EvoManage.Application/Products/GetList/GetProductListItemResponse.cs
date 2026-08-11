using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.GetList;

public sealed record GetProductListItemResponse(
    int Id,
    string Code,
    string Name,
    ProductTrackingType TrackingType,
    bool IsActive);