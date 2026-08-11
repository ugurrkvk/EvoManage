using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Deactivate;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Products.Deactivate;

public sealed class DeactivateProductServiceTests
{
    [Fact]
    public async Task DeactivateAsync_WithExistingProduct_ShouldDeactivateAndSaveChanges()
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

        var service = new DeactivateProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        await service.DeactivateAsync(1);

        // Assert
        Assert.False(product.IsActive);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new DeactivateProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var act = () => service.DeactivateAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}