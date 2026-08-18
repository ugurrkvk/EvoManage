using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;
using EvoManage.Domain.Locations;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Validation.Common.Validators;

public sealed class WarehouseLocationValidatorTests
{
    private readonly Mock<ILocationRepository> _locationRepository = new();

    private WarehouseLocationValidator CreateValidator()
        => new(_locationRepository.Object);

    [Fact]
    public async Task ValidateAsync_WithValidLocation_ShouldReturnLocation()
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

        var validator = CreateValidator();

        // Act
        var result = await validator.ValidateAsync(
            locationId: 10,
            warehouseId: 1);

        // Assert
        Assert.Same(location, result);
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

        var validator = CreateValidator();

        // Act
        var act = () => validator.ValidateAsync(
            locationId: 999,
            warehouseId: 1);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
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

        var validator = CreateValidator();

        // Act
        var act = () => validator.ValidateAsync(
            locationId: 10,
            warehouseId: 1);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
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

        var validator = CreateValidator();

        // Act
        var act = () => validator.ValidateAsync(
            locationId: 10,
            warehouseId: 1);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }
}