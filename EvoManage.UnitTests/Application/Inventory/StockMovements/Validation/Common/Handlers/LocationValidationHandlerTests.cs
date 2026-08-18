using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Handlers;
using EvoManage.Application.Inventory.StockMovements.Validation.Common;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;
using EvoManage.Domain.Locations;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Common.Handlers;

public sealed class LocationValidationHandlerTests
{
    private readonly Mock<ILocationRepository> _locationRepository = new();

    private LocationValidationHandler CreateHandler()
        => new LocationValidationHandler(
            new WarehouseLocationValidator(_locationRepository.Object));

    [Fact]
    public async Task ValidateAsync_WithNullLocationId_ShouldSkipValidation()
    {
        // Arrange
        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: null,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        await handler.ValidateAsync(context);

        // Assert
        Assert.Null(context.Location);

        _locationRepository.Verify(
            repository => repository.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidateAsync_WithValidLocation_ShouldSetLocationOnContext()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: 10,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        await handler.ValidateAsync(context);

        // Assert
        Assert.Same(location, context.Location);
    }

    [Fact]
    public async Task ValidateAsync_WithMissingLocation_ShouldThrowNotFoundException()
    {
        // Arrange
        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        var context = new StockMovementValidationContext(
            productId: 1,
            warehouseId: 1,
            locationId: 999,
            quantity: 5m);

        var handler = CreateHandler();

        // Act
        var act = () => handler.ValidateAsync(context);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Null(context.Location);
    }

    [Fact]
    public async Task ValidateAsync_WithInactiveLocation_ShouldThrowConflictException()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        location.Deactivate();

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

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
        Assert.Null(context.Location);
    }

    [Fact]
    public async Task ValidateAsync_WithLocationBelongingToDifferentWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 2,
            code: "A-01-01");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

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
        Assert.Null(context.Location);
    }
}