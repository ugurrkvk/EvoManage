using EvoManage.Application.Inventory.StockMovements.Events;
using EvoManage.Domain.Inventory.StockMovements;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Events;

public sealed class StockMovementCreatedEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_ShouldCallAllRegisteredHandlers()
    {
        // Arrange
        var firstHandler = new Mock<IStockMovementCreatedEventHandler>();
        var secondHandler = new Mock<IStockMovementCreatedEventHandler>();

        var dispatcher = new StockMovementCreatedEventDispatcher(
        [
            firstHandler.Object,
            secondHandler.Object
        ]);

        var @event = new StockMovementCreatedEvent(
            MovementId: 10,
            ProductId: 1,
            WarehouseId: 2,
            LocationId: 3,
            Quantity: 25m,
            MovementType: StockMovementType.Issue);

        // Act
        await dispatcher.DispatchAsync(@event);

        // Assert
        firstHandler.Verify(
            handler => handler.HandleAsync(
                @event,
                It.IsAny<CancellationToken>()),
            Times.Once);

        secondHandler.Verify(
            handler => handler.HandleAsync(
                @event,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithNoHandlers_ShouldCompleteSuccessfully()
    {
        // Arrange
        var dispatcher = new StockMovementCreatedEventDispatcher([]);

        var @event = new StockMovementCreatedEvent(
            MovementId: 10,
            ProductId: 1,
            WarehouseId: 2,
            LocationId: 3,
            Quantity: 25m,
            MovementType: StockMovementType.Issue);

        // Act
        var exception = await Record.ExceptionAsync(
            () => dispatcher.DispatchAsync(@event));

        // Assert
        Assert.Null(exception);
    }
}