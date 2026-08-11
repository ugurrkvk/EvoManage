using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Products.GetById;

public sealed class GetProductByIdService(
    IProductRepository productRepository)
{
    public async Task<GetProductByIdResponse> GetAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            throw new NotFoundException(
                $"Product with id '{id}' was not found.");

        return new GetProductByIdResponse(
            product.Id,
            product.Code,
            product.Name,
            product.TrackingType,
            product.IsActive);
    }
}