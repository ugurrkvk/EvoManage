using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.Common.StockAllocation.Strategies;
using EvoManage.Domain.Locations;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.Common.StockAllocation;

public sealed class ManualLocationAllocationStrategyTests
{
    private readonly Mock<IStockReadRepository> _stockReadRepository = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();

    private ManualLocationAllocationStrategy CreateStrategy()
        => new(
            _stockReadRepository.Object,
            _locationRepository.Object);

    [Fact]
    public async Task AllocateAsync_WithValidLocationAndSufficientStock_ShouldReturnAllocation()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _stockReadRepository
            .Setup(repository => repository.GetBalanceAsync(
                1,
                1,
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(25m);

        var strategy = CreateStrategy();

        // Act
        var allocations = await strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: 1,
            quantity: 10m);

        // Assert
        var allocation = Assert.Single(allocations);

        Assert.Equal(1, allocation.WarehouseId);
        Assert.Equal(1, allocation.LocationId);
        Assert.Equal(10m, allocation.Quantity);
    }

    [Fact]
    public async Task AllocateAsync_WithMissingRequestedLocation_ShouldThrowConflictException()
    {
        // Arrange
        var strategy = CreateStrategy();

        // Act
        var act = () => strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: null,
            quantity: 10m);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task AllocateAsync_WithMissingLocation_ShouldThrowNotFoundException()
    {
        // Arrange
        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        var strategy = CreateStrategy();

        // Act
        var act = () => strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: 999,
            quantity: 10m);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task AllocateAsync_WithInactiveLocation_ShouldThrowConflictException()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        location.Deactivate();

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        var strategy = CreateStrategy();

        // Act
        var act = () => strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: 1,
            quantity: 10m);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task AllocateAsync_WithLocationBelongingToDifferentWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 2,
            code: "A-01-01");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        var strategy = CreateStrategy();

        // Act
        var act = () => strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: 1,
            quantity: 10m);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task AllocateAsync_WithInsufficientStock_ShouldThrowConflictException()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _stockReadRepository
            .Setup(repository => repository.GetBalanceAsync(
                1,
                1,
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(5m);

        var strategy = CreateStrategy();

        // Act
        var act = () => strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: 1,
            quantity: 10m);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    [Fact]
    public async Task AllocateAsync_WithExactStock_ShouldReturnAllocation()
    {
        // Arrange
        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _stockReadRepository
            .Setup(repository => repository.GetBalanceAsync(
                1,
                1,
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(10m);

        var strategy = CreateStrategy();

        // Act
        var allocations = await strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: 1,
            quantity: 10m);

        // Assert
        var allocation = Assert.Single(allocations);

        Assert.Equal(10m, allocation.Quantity);
    }

    private void VerifyStockWasNotQueried()
    {
        _stockReadRepository.Verify(
            repository => repository.GetBalanceAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}