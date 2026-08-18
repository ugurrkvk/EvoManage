using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Common.Validators;

public sealed class ActiveWarehouseValidatorTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();

    private ActiveWarehouseValidator CreateValidator()
        => new(_warehouseRepository.Object);

    [Fact]
    public async Task ValidateAsync_WithValidWarehouse_ShouldReturnWarehouse()
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

        var validator = CreateValidator();

        // Act
        var result = await validator.ValidateAsync(1);

        // Assert
        Assert.Same(warehouse, result);
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

        var validator = CreateValidator();

        // Act
        var act = () => validator.ValidateAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
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

        var validator = CreateValidator();

        // Act
        var act = () => validator.ValidateAsync(1);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }
}