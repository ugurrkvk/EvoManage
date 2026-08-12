using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Commands.Create;
using EvoManage.Application.Products.Commands.Update;
using EvoManage.Domain.Products;

namespace EvoManage.Application.Products.Commands;

public class ProductCommandService(IProductRepository productRepository, IUnitOfWork unitOfWork)
{
    public async Task<CreateProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await productRepository.ExistsByCodeAsync(request.Code, cancellationToken);
        if (exists) throw new ConflictException($"Product code '{request.Code}' already exists.");

        var product = Product.Create(
            request.Code,
            request.Name,
            request.TrackingType);

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateProductResponse(product.Id);
    }

    public async Task UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null) throw new NotFoundException($"Product with id '{id}' was not found.");

        var codeExists = await productRepository.ExistsByCodeExceptIdAsync(request.Code, id, cancellationToken);
        if (codeExists) throw new ConflictException($"Product code '{request.Code}' already exists.");

        product.Update(
            request.Code,
            request.Name,
            request.TrackingType);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null) throw new NotFoundException($"Product with id '{id}' was not found.");

        productRepository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrThrowAsync(id, cancellationToken);

        product.Activate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrThrowAsync(id, cancellationToken);

        product.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Product> GetProductOrThrowAsync(int id, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetByIdAsync(id, cancellationToken)
               ?? throw new NotFoundException($"Product with id '{id}' was not found.");
    }

}