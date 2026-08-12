using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.Commands.Create;

public sealed record CreateProductRequest(string Code, string Name, ProductTrackingType TrackingType);