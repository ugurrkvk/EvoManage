using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.Update;

public sealed record UpdateProductRequest(
    string Code,
    string Name,
    ProductTrackingType TrackingType);