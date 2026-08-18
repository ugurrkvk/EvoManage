using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Transfer;
using EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Transfer.Handlers;

public sealed class DifferentSourceAndTargetValidationHandlerTests
{
    private readonly DifferentSourceAndTargetValidationHandler _handler = new();

    [Fact]
    public async Task ValidateAsync_WithDifferentSourceAndTarget_ShouldSucceed()
    {
        // Arrange
        var context = new StockTransferValidationContext(
            productId: 1,
            sourceWarehouseId: 1,
            sourceLocationId: 10,
            targetWarehouseId: 2,
            targetLocationId: 20,
            quantity: 5m);

        // Act
        await _handler.ValidateAsync(context);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task ValidateAsync_WithSameSourceAndTarget_ShouldThrowConflictException()
    {
        // Arrange
        var context = new StockTransferValidationContext(
            productId: 1,
            sourceWarehouseId: 1,
            sourceLocationId: 10,
            targetWarehouseId: 1,
            targetLocationId: 10,
            quantity: 5m);

        // Act
        var act = () => _handler.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    [Fact]
    public async Task ValidateAsync_WithSameLocationIdButDifferentWarehouse_ShouldSucceed()
    {
        // Arrange
        var context = new StockTransferValidationContext(
            productId: 1,
            sourceWarehouseId: 1,
            sourceLocationId: 10,
            targetWarehouseId: 2,
            targetLocationId: 10,
            quantity: 5m);

        // Act
        await _handler.ValidateAsync(context);

        // Assert
        Assert.True(true);
    }
}