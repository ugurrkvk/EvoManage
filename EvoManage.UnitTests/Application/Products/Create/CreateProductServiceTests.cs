using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Products.Create;
using EvoManage.Domain.Products;
using Moq;
using EvoManage.Application.Common.Exceptions;

namespace EvoManage.UnitTests.Application.Products.Create;

public sealed class CreateProductServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldAddProductAndSaveChanges()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new CreateProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new CreateProductRequest(
            "PRD-001",
            "Test Product",
            ProductTrackingType.Lot);

        // Act
        var response = await service.CreateAsync(request);

        // Assert
        productRepository.Verify(
            repository => repository.AddAsync(
                It.Is<Product>(product =>
                    product.Code == request.Code &&
                    product.Name == request.Name &&
                    product.TrackingType == request.TrackingType),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(response);
    }

    [Fact]
    public async Task CreateAsync_WithExistingCode_ShouldThrowConflictException()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        productRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new CreateProductService(
            productRepository.Object,
            unitOfWork.Object);

        var request = new CreateProductRequest(
            "PRD-001",
            "Test Product",
            ProductTrackingType.Lot);

        // Act
        var act = () => service.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        productRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}