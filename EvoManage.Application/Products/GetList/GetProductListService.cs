using EvoManage.Application.Abstractions.Persistence.Repositories;

namespace EvoManage.Application.Products.GetList;

public sealed class GetProductListService(
    IProductRepository productRepository)
{
    public async Task<GetProductListResponse> GetAsync(
        GetProductListRequest request,
        CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalCount = await productRepository.CountAsync(
            cancellationToken);

        var items = products
            .Select(product => new GetProductListItemResponse(
                product.Id,
                product.Code,
                product.Name,
                product.TrackingType,
                product.IsActive))
            .ToArray();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)request.PageSize);

        return new GetProductListResponse(
            items,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages);
    }
}