using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.Application.Products.Activate;

public sealed class ActivateProductService(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
{
    public async Task ActivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
            throw new NotFoundException(
                $"Product with id '{id}' was not found.");

        product.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}