using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Update;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Products.Update;

public sealed class UpdateProductServiceTests
{
    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldUpdateProductAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-003",
            "Old Product",
            ProductTrackingType.None);

        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        productRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                "PRD-003",
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new UpdateProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new UpdateProductRequest(
            "PRD-003",
            "Updated Product",
            ProductTrackingType.Lot);

        // Act
        await service.UpdateAsync(3, request);

        // Assert
        Assert.Equal("Updated Product", product.Name);
        Assert.Equal(ProductTrackingType.Lot, product.TrackingType);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new UpdateProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new UpdateProductRequest(
            "PRD-999",
            "Missing Product",
            ProductTrackingType.None);

        // Act
        var act = () => service.UpdateAsync(999, request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingCode_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-003",
            "Old Product",
            ProductTrackingType.None);

        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        productRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                "PRD-001",
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new UpdateProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new UpdateProductRequest(
            "PRD-001",
            "Updated Product",
            ProductTrackingType.Lot);

        // Act
        var act = () => service.UpdateAsync(3, request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}