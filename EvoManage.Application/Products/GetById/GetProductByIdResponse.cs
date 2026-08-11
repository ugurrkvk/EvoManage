using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.GetById;

public sealed record GetProductByIdResponse(
    int Id,
    string Code,
    string Name,
    ProductTrackingType TrackingType,
    bool IsActive);