using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.Create;

public sealed class CreateProductService(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var exists = await productRepository.ExistsByCodeAsync(
            request.Code,
            cancellationToken);

        if (exists)
            throw new ConflictException(
                $"Product code '{request.Code}' already exists.");

        var product = Product.Create(
            request.Code,
            request.Name,
            request.TrackingType);

        await productRepository.AddAsync(
            product,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id);
    }
}