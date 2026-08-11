using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Activate;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Products.Activate;

public sealed class ActivateProductServiceTests
{
    [Fact]
    public async Task ActivateAsync_WithExistingProduct_ShouldActivateAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        product.Deactivate();

        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var service = new ActivateProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        await service.ActivateAsync(1);

        // Assert
        Assert.True(product.IsActive);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new ActivateProductService(
            productRepository.Object,
            unitOfWork.Object);

        // Act
        var act = () => service.ActivateAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}