using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.Commands.Update;

public sealed record UpdateProductRequest(
    string Code,
    string Name,
    ProductTrackingType TrackingType);