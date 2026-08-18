using EvoManage.Application.Inventory.StockMovements.Validation.Common;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Common;

public sealed class StockMovementValidationPipelineTest
{
    [Fact]
    public async Task ValidateAsync_ShouldExecuteHandlersByStepOrder()
    {
        // Arrange
        var executionOrder = new List<StockMovementValidationStep>();

        var productHandler = new Mock<IStockMovementValidationHandler>();
        var warehouseHandler = new Mock<IStockMovementValidationHandler>();
        var locationHandler = new Mock<IStockMovementValidationHandler>();

        productHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockMovementValidationStep.Product);

        warehouseHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockMovementValidationStep.Warehouse);

        locationHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockMovementValidationStep.Location);

        productHandler
            .Setup(handler => handler.ValidateAsync(
                It.IsAny<StockMovementValidationContext>(),
                It.IsAny<CancellationToken>()))
            .Callback(() =>
                executionOrder.Add(StockMovementValidationStep.Product))
            .Returns(Task.CompletedTask);

        warehouseHandler
            .Setup(handler => handler.ValidateAsync(
                It.IsAny<StockMovementValidationContext>(),
                It.IsAny<CancellationToken>()))
            .Callback(() =>
                executionOrder.Add(StockMovementValidationStep.Warehouse))
            .Returns(Task.CompletedTask);

        locationHandler
            .Setup(handler => handler.ValidateAsync(
                It.IsAny<StockMovementValidationContext>(),
                It.IsAny<CancellationToken>()))
            .Callback(() =>
                executionOrder.Add(StockMovementValidationStep.Location))
            .Returns(Task.CompletedTask);

        var pipeline = new StockMovementValidationPipeline(
        [
            locationHandler.Object,
            productHandler.Object,
            warehouseHandler.Object
        ]);

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: 10,
            quantity: 5m);

        // Act
        await pipeline.ValidateAsync(context);

        // Assert
        Assert.Equal(
        [
            StockMovementValidationStep.Product,
            StockMovementValidationStep.Warehouse,
            StockMovementValidationStep.Location
        ],
        executionOrder);
    }

    [Fact]
    public async Task ValidateAsync_WhenHandlerThrows_ShouldStopPipeline()
    {
        // Arrange
        var productHandler = new Mock<IStockMovementValidationHandler>();
        var warehouseHandler = new Mock<IStockMovementValidationHandler>();
        var locationHandler = new Mock<IStockMovementValidationHandler>();

        productHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockMovementValidationStep.Product);

        warehouseHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockMovementValidationStep.Warehouse);

        locationHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockMovementValidationStep.Location);

        productHandler
            .Setup(handler => handler.ValidateAsync(
                It.IsAny<StockMovementValidationContext>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        warehouseHandler
            .Setup(handler => handler.ValidateAsync(
                It.IsAny<StockMovementValidationContext>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Validation failed."));

        var pipeline = new StockMovementValidationPipeline(
        [
            productHandler.Object,
            warehouseHandler.Object,
            locationHandler.Object
        ]);

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: 10,
            quantity: 5m);

        // Act
        var act = () => pipeline.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        productHandler.Verify(
            handler => handler.ValidateAsync(
                context,
                It.IsAny<CancellationToken>()),
            Times.Once);

        warehouseHandler.Verify(
            handler => handler.ValidateAsync(
                context,
                It.IsAny<CancellationToken>()),
            Times.Once);

        locationHandler.Verify(
            handler => handler.ValidateAsync(
                It.IsAny<StockMovementValidationContext>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}