using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Queries.GetById;
using EvoManage.Application.Products.Queries.GetList;

namespace EvoManage.Application.Products.Queries;

public class ProductQueryService(IProductRepository productRepository)
{
    public async Task<GetProductByIdResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null) throw new NotFoundException($"Product with id '{id}' was not found.");

        return new GetProductByIdResponse(
            product.Id,
            product.Code,
            product.Name,
            product.TrackingType,
            product.IsActive);
    }

    public async Task<GetProductListResponse> GetListAsync(GetProductListRequest request, CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var totalCount = await productRepository.CountAsync(cancellationToken);

        var items = products
            .Select(product => new GetProductListItemResponse(
                product.Id,
                product.Code,
                product.Name,
                product.TrackingType,
                product.IsActive))
            .ToArray();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new GetProductListResponse(
            items,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages);
    }
}