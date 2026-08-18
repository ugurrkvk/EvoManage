using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Common;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Handlers;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Common.Handlers;

public sealed class WarehouseValidationHandlerTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();

    private WarehouseValidationHandler CreateHandler()
        => new WarehouseValidationHandler(
            new ActiveWarehouseValidator(_warehouseRepository.Object));

    [Fact]
    public async Task ValidateAsync_WithValidWarehouse_ShouldSetWarehouseOnContext()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: 10,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        await handler.ValidateAsync(context);

        // Assert
        Assert.Same(warehouse, context.Warehouse);
    }

    [Fact]
    public async Task ValidateAsync_WithMissingWarehouse_ShouldThrowNotFoundException()
    {
        // Arrange
        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 999,
            locationId: 10,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        var act = () => handler.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Null(context.Warehouse);
    }

    [Fact]
    public async Task ValidateAsync_WithInactiveWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        warehouse.Deactivate();

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

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
        Assert.Null(context.Warehouse);
    }
}