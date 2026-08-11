using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Products.Delete;

public sealed class DeleteProductService(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
{
    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            throw new NotFoundException(
                $"Product with id '{id}' was not found.");

        productRepository.Remove(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}