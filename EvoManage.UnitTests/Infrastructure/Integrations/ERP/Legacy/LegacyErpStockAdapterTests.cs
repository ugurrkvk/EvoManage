using EvoManage.Application.Integrations.ERP.Stock;
using EvoManage.Domain.Inventory.StockMovements;
using EvoManage.Infrastructure.Integrations.ERP.Legacy;
using Moq;

namespace EvoManage.UnitTests.Infrastructure.Integrations.ERP.Legacy;

public sealed class LegacyErpStockAdapterTests
{
    private readonly Mock<ILegacyErpClient> _legacyErpClient = new();

    private LegacyErpStockAdapter CreateAdapter()
        => new(_legacyErpClient.Object);

    [Theory]
    [InlineData(StockMovementType.Receipt, "IN")]
    [InlineData(StockMovementType.Issue, "OUT")]
    [InlineData(StockMovementType.TransferIn, "TRANSFER_IN")]
    [InlineData(StockMovementType.TransferOut, "TRANSFER_OUT")]
    public async Task SendStockMovementAsync_WithSupportedMovementType_ShouldMapAndSendLegacyRequest(
        StockMovementType movementType,
        string expectedTransactionType)
    {
        // Arrange
        var movement = new ErpStockMovementModel(
            ProductId: 15,
            WarehouseId: 3,
            LocationId: 10,
            Quantity: 25.5m,
            MovementType: movementType);

        var adapter = CreateAdapter();

        // Act
        await adapter.SendStockMovementAsync(movement);

        // Assert
        _legacyErpClient.Verify(
            client => client.SendStockTransactionAsync(
                It.Is<LegacyErpStockRequest>(request =>
                    request.ItemCode == "15" &&
                    request.WarehouseNumber == 3 &&
                    request.TransactionAmount == 25.5m &&
                    request.TransactionType == expectedTransactionType),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendStockMovementAsync_WithUnsupportedMovementType_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var movement = new ErpStockMovementModel(
            ProductId: 15,
            WarehouseId: 3,
            LocationId: 10,
            Quantity: 25.5m,
            MovementType: (StockMovementType)999);

        var adapter = CreateAdapter();

        // Act
        var act = () => adapter.SendStockMovementAsync(movement);

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(act);

        _legacyErpClient.Verify(
            client => client.SendStockTransactionAsync(
                It.IsAny<LegacyErpStockRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SendStockMovementAsync_WithWarehouseIdOutsideShortRange_ShouldThrowOverflowException()
    {
        // Arrange
        var movement = new ErpStockMovementModel(
            ProductId: 15,
            WarehouseId: short.MaxValue + 1,
            LocationId: 10,
            Quantity: 25.5m,
            MovementType: StockMovementType.Issue);

        var adapter = CreateAdapter();

        // Act
        var act = () => adapter.SendStockMovementAsync(movement);

        // Assert
        await Assert.ThrowsAsync<OverflowException>(act);

        _legacyErpClient.Verify(
            client => client.SendStockTransactionAsync(
                It.IsAny<LegacyErpStockRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}