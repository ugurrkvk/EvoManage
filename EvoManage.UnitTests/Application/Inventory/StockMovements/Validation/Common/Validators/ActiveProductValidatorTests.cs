using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Common.Validators;

public sealed class ActiveProductValidatorTests
{
    private readonly Mock<IProductRepository> _productRepository = new();

    private ActiveProductValidator CreateValidator()
        => new(_productRepository.Object);

    [Fact]
    public async Task ValidateAsync_WithValidProduct_ShouldReturnProduct()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var validator = CreateValidator();

        // Act
        var result = await validator.ValidateAsync(1);

        // Assert
        Assert.Same(product, result);
    }

    [Fact]
    public async Task ValidateAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var validator = CreateValidator();

        // Act
        var act = () => validator.ValidateAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task ValidateAsync_WithInactiveProduct_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        product.Deactivate();

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var validator = CreateValidator();

        // Act
        var act = () => validator.ValidateAsync(1);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }
}