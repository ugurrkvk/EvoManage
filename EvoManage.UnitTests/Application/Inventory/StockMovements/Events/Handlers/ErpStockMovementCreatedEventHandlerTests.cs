using EvoManage.Application.Integrations.ERP.Stock;
using EvoManage.Application.Inventory.StockMovements.Events;
using EvoManage.Application.Inventory.StockMovements.Events.Handlers;
using EvoManage.Domain.Inventory.StockMovements;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Events.Handlers;

public sealed class ErpStockMovementCreatedEventHandlerTests
{
    private readonly Mock<IErpStockIntegration> _erpStockIntegration = new();

    private ErpStockMovementCreatedEventHandler CreateHandler()
        => new(_erpStockIntegration.Object);

    [Fact]
    public async Task HandleAsync_ShouldMapEventAndSendStockMovementToErpIntegration()
    {
        // Arrange
        var @event = new StockMovementCreatedEvent(
            MovementId: 25,
            ProductId: 1,
            WarehouseId: 2,
            LocationId: 3,
            Quantity: 15.5m,
            MovementType: StockMovementType.Issue);

        var handler = CreateHandler();

        // Act
        await handler.HandleAsync(@event);

        // Assert
        _erpStockIntegration.Verify(
            integration => integration.SendStockMovementAsync(
                It.Is<ErpStockMovementModel>(movement =>
                    movement.ProductId == 1 &&
                    movement.WarehouseId == 2 &&
                    movement.LocationId == 3 &&
                    movement.Quantity == 15.5m &&
                    movement.MovementType == StockMovementType.Issue),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(StockMovementType.Receipt)]
    [InlineData(StockMovementType.Issue)]
    [InlineData(StockMovementType.TransferIn)]
    [InlineData(StockMovementType.TransferOut)]
    public async Task HandleAsync_ShouldPreserveMovementType(
        StockMovementType movementType)
    {
        // Arrange
        var @event = new StockMovementCreatedEvent(
            MovementId: 25,
            ProductId: 1,
            WarehouseId: 2,
            LocationId: 3,
            Quantity: 10m,
            MovementType: movementType);

        var handler = CreateHandler();

        // Act
        await handler.HandleAsync(@event);

        // Assert
        _erpStockIntegration.Verify(
            integration => integration.SendStockMovementAsync(
                It.Is<ErpStockMovementModel>(movement =>
                    movement.MovementType == movementType),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}