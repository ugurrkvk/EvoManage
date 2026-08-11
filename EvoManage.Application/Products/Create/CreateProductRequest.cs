using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.Create;

public sealed record CreateProductRequest(string Code, string Name, ProductTrackingType TrackingType);