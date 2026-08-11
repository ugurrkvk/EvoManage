using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Delete;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Products.Delete;

public sealed class DeleteProductServiceTests
{
    [Fact]
    public async Task DeleteAsync_WithExistingProduct_ShouldRemoveProductAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var service = new DeleteProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        await service.DeleteAsync(1);

        // Assert
        productRepository.Verify(
            repository => repository.Remove(product),
            Times.Once);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new DeleteProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var act = () => service.DeleteAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        productRepository.Verify(
            repository => repository.Remove(It.IsAny<Product>()),
            Times.Never);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}