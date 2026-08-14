using EvoManage.Domain.Common.Exceptions;
using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.UnitTests.Domain.Inventory.StockMovements;

public sealed class StockMovementTests
{
    [Theory]
    [InlineData(StockMovementType.Receipt)]
    [InlineData(StockMovementType.Issue)]
    [InlineData(StockMovementType.TransferIn)]
    [InlineData(StockMovementType.TransferOut)]
    public void Create_WithValidValues_ShouldCreateStockMovement(
        StockMovementType movementType)
    {
        // Act
        var movement = StockMovement.Create(
            productId: 1,
            warehouseId: 2,
            locationId: 3,
            quantity: 10,
            movementType);

        // Assert
        Assert.Equal(1, movement.ProductId);
        Assert.Equal(2, movement.WarehouseId);
        Assert.Equal(3, movement.LocationId);
        Assert.Equal(10, movement.Quantity);
        Assert.Equal(movementType, movement.MovementType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidProductId_ShouldThrowDomainException(
        int productId)
    {
        var act = () => StockMovement.Create(
            productId,
            1,
            1,
            10,
            StockMovementType.Receipt);

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidWarehouseId_ShouldThrowDomainException(
        int warehouseId)
    {
        var act = () => StockMovement.Create(
            1,
            warehouseId,
            1,
            10,
            StockMovementType.Receipt);

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidLocationId_ShouldThrowDomainException(
        int locationId)
    {
        var act = () => StockMovement.Create(
            1,
            1,
            locationId,
            10,
            StockMovementType.Receipt);

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void Create_WithInvalidQuantity_ShouldThrowDomainException(
        decimal quantity)
    {
        var act = () => StockMovement.Create(
            1,
            1,
            1,
            quantity,
            StockMovementType.Receipt);

        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void Create_WithInvalidMovementType_ShouldThrowDomainException(
        int movementType)
    {
        var act = () => StockMovement.Create(
            1,
            1,
            1,
            10,
            (StockMovementType)movementType);

        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithDecimalQuantity_ShouldPreserveQuantity()
    {
        var movement = StockMovement.Create(
            1,
            1,
            1,
            10.75m,
            StockMovementType.Receipt);

        Assert.Equal(10.75m, movement.Quantity);
    }

    [Theory]
    [InlineData(StockMovementType.Receipt, 10)]
    [InlineData(StockMovementType.TransferIn, 10)]
    [InlineData(StockMovementType.Issue, -10)]
    [InlineData(StockMovementType.TransferOut, -10)]
    public void SignedQuantity_ShouldReturnQuantityWithCorrectDirection(
        StockMovementType movementType,
        decimal expectedQuantity)
    {
        // Arrange
        var movement = StockMovement.Create(
            1,
            1,
            1,
            10,
            movementType);

        // Act
        var signedQuantity = movement.SignedQuantity;

        // Assert
        Assert.Equal(expectedQuantity, signedQuantity);
    }
}