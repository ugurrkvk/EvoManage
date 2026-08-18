using EvoManage.Application.Inventory.StockMovements.Validation.Transfer;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Transfer;

public sealed class StockTransferValidationPipelineTests
{
    [Fact]
    public async Task ValidateAsync_ShouldExecuteHandlersByStepOrder()
    {
        // Arrange
        var executionOrder = new List<StockTransferValidationStep>();

        var productHandler = new Mock<IStockTransferValidationHandler>();
        var sourceWarehouseHandler = new Mock<IStockTransferValidationHandler>();
        var sourceLocationHandler = new Mock<IStockTransferValidationHandler>();
        var targetWarehouseHandler = new Mock<IStockTransferValidationHandler>();
        var targetLocationHandler = new Mock<IStockTransferValidationHandler>();
        var differentSourceTargetHandler = new Mock<IStockTransferValidationHandler>();
        var sufficientStockHandler = new Mock<IStockTransferValidationHandler>();

        SetupHandler(
            productHandler,
            StockTransferValidationStep.Product,
            executionOrder);

        SetupHandler(
            sourceWarehouseHandler,
            StockTransferValidationStep.SourceWarehouse,
            executionOrder);

        SetupHandler(
            sourceLocationHandler,
            StockTransferValidationStep.SourceLocation,
            executionOrder);

        SetupHandler(
            targetWarehouseHandler,
            StockTransferValidationStep.TargetWarehouse,
            executionOrder);

        SetupHandler(
            targetLocationHandler,
            StockTransferValidationStep.TargetLocation,
            executionOrder);

        SetupHandler(
            differentSourceTargetHandler,
            StockTransferValidationStep.DifferentSourceTarget,
            executionOrder);

        SetupHandler(
            sufficientStockHandler,
            StockTransferValidationStep.SufficientStock,
            executionOrder);

        var pipeline = new StockTransferValidationPipeline(
        [
            sufficientStockHandler.Object,
            targetLocationHandler.Object,
            productHandler.Object,
            sourceLocationHandler.Object,
            differentSourceTargetHandler.Object,
            sourceWarehouseHandler.Object,
            targetWarehouseHandler.Object
        ]);

        var context = CreateContext();

        // Act
        await pipeline.ValidateAsync(context);

        // Assert
        Assert.Equal(
        [
            StockTransferValidationStep.Product,
            StockTransferValidationStep.SourceWarehouse,
            StockTransferValidationStep.SourceLocation,
            StockTransferValidationStep.TargetWarehouse,
            StockTransferValidationStep.TargetLocation,
            StockTransferValidationStep.DifferentSourceTarget,
            StockTransferValidationStep.SufficientStock
        ],
        executionOrder);
    }

    [Fact]
    public async Task ValidateAsync_WhenHandlerThrows_ShouldStopPipeline()
    {
        // Arrange
        var productHandler = new Mock<IStockTransferValidationHandler>();
        var sourceWarehouseHandler = new Mock<IStockTransferValidationHandler>();
        var sourceLocationHandler = new Mock<IStockTransferValidationHandler>();

        productHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockTransferValidationStep.Product);

        sourceWarehouseHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockTransferValidationStep.SourceWarehouse);

        sourceLocationHandler
            .SetupGet(handler => handler.Step)
            .Returns(StockTransferValidationStep.SourceLocation);

        productHandler
            .Setup(handler => handler.ValidateAsync(
                It.IsAny<StockTransferValidationContext>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        sourceWarehouseHandler
            .Setup(handler => handler.ValidateAsync(
                It.IsAny<StockTransferValidationContext>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Validation failed."));

        var pipeline = new StockTransferValidationPipeline(
        [
            sourceLocationHandler.Object,
            sourceWarehouseHandler.Object,
            productHandler.Object
        ]);

        var context = CreateContext();

        // Act
        var act = () => pipeline.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        productHandler.Verify(
            handler => handler.ValidateAsync(
                context,
                It.IsAny<CancellationToken>()),
            Times.Once);

        sourceWarehouseHandler.Verify(
            handler => handler.ValidateAsync(
                context,
                It.IsAny<CancellationToken>()),
            Times.Once);

        sourceLocationHandler.Verify(
            handler => handler.ValidateAsync(
                It.IsAny<StockTransferValidationContext>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static void SetupHandler(
        Mock<IStockTransferValidationHandler> handler,
        StockTransferValidationStep step,
        ICollection<StockTransferValidationStep> executionOrder)
    {
        handler
            .SetupGet(currentHandler => currentHandler.Step)
            .Returns(step);

        handler
            .Setup(currentHandler => currentHandler.ValidateAsync(
                It.IsAny<StockTransferValidationContext>(),
                It.IsAny<CancellationToken>()))
            .Callback(() => executionOrder.Add(step))
            .Returns(Task.CompletedTask);
    }

    private static StockTransferValidationContext CreateContext()
        => new(
            productId: 1,
            sourceWarehouseId: 1,
            sourceLocationId: 10,
            targetWarehouseId: 2,
            targetLocationId: 20,
            quantity: 5m);
}