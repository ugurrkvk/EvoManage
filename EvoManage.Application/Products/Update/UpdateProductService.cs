using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Products.Update;

public sealed class UpdateProductService(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
{
    public async Task UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            throw new NotFoundException(
                $"Product with id '{id}' was not found.");

        var codeExists = await productRepository.ExistsByCodeExceptIdAsync(
            request.Code,
            id,
            cancellationToken);

        if (codeExists)
            throw new ConflictException(
                $"Product code '{request.Code}' already exists.");

        product.Update(
            request.Code,
            request.Name,
            request.TrackingType);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}