using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Common;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Handlers;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Common.Handlers;

public sealed class ProductValidationHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();

    private ProductValidationHandler CreateHandler()
        => new ProductValidationHandler(
            new ActiveProductValidator(_productRepository.Object));

    [Fact]
    public async Task ValidateAsync_WithValidProduct_ShouldSetProductOnContext()
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

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: 10,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        await handler.ValidateAsync(context);

        // Assert
        Assert.Same(product, context.Product);
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

        var context = new StockMovementValidationContext(
            productId: 999,
            warehouseId: 1,
            locationId: 10,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        var act = () => handler.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Null(context.Product);
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

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: 10,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        var act = () => handler.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
        Assert.Null(context.Product);
    }
}