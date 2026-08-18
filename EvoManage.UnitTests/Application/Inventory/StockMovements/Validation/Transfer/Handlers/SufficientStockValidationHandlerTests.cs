using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Transfer;
using EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class SufficientStockValidationHandlerTests
{
    private readonly Mock<IStockMovementRepository> _stockMovementRepository = new();

    private SufficientStockValidationHandler CreateHandler()
        => new(_stockMovementRepository.Object);

    [Fact]
    public async Task ValidateAsync_WithSufficientStock_ShouldSucceed()
    {
        // Arrange
        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(10m);

        var context = CreateContext(quantity: 5m);

        var handler = CreateHandler();

        // Act
        await handler.ValidateAsync(context);

        // Assert
        _stockMovementRepository.Verify(
            repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_WithExactStock_ShouldSucceed()
    {
        // Arrange
        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(5m);

        var context = CreateContext(quantity: 5m);

        var handler = CreateHandler();

        // Act
        await handler.ValidateAsync(context);

        // Assert
        _stockMovementRepository.Verify(
            repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_WithInsufficientStock_ShouldThrowConflictException()
    {
        // Arrange
        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(3m);

        var context = CreateContext(quantity: 5m);

        var handler = CreateHandler();

        // Act
        var act = () => handler.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    private static StockTransferValidationContext CreateContext(decimal quantity)
        => new(
            productId: 1,
            sourceWarehouseId: 1,
            sourceLocationId: 10,
            targetWarehouseId: 2,
            targetLocationId: 20,
            quantity: quantity);
}