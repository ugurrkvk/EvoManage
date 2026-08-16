using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.Common.StockAllocation.Strategies;
using EvoManage.Application.Inventory.Stocks.Models;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.Common.StockAllocation;

public sealed class HighestStockAllocationStrategyTests
{
    private readonly Mock<IStockReadRepository> _stockReadRepository = new();

    private HighestStockAllocationStrategy CreateStrategy()
        => new(_stockReadRepository.Object);

    [Fact]
    public async Task AllocateAsync_WithSingleLocationHavingEnoughStock_ShouldReturnSingleAllocation()
    {
        // Arrange
        SetupAvailableStocks(
        [
            CreateStock(
                locationId: 10,
                locationCode: "A-01-01",
                quantity: 40m),

            CreateStock(
                locationId: 20,
                locationCode: "A-01-02",
                quantity: 25m)
        ]);

        var strategy = CreateStrategy();

        // Act
        var allocations = await strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: null,
            quantity: 30m);

        // Assert
        var allocation = Assert.Single(allocations);

        Assert.Equal(1, allocation.WarehouseId);
        Assert.Equal(10, allocation.LocationId);
        Assert.Equal(30m, allocation.Quantity);
    }

    [Fact]
    public async Task AllocateAsync_WithQuantitySpanningMultipleLocations_ShouldReturnMultipleAllocations()
    {
        // Arrange
        SetupAvailableStocks(
        [
            CreateStock(
                locationId: 10,
                locationCode: "A-01-01",
                quantity: 40m),

            CreateStock(
                locationId: 20,
                locationCode: "A-01-02",
                quantity: 25m),

            CreateStock(
                locationId: 30,
                locationCode: "A-01-03",
                quantity: 10m)
        ]);

        var strategy = CreateStrategy();

        // Act
        var allocations = await strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: null,
            quantity: 50m);

        // Assert
        Assert.Collection(
            allocations,
            first =>
            {
                Assert.Equal(1, first.WarehouseId);
                Assert.Equal(10, first.LocationId);
                Assert.Equal(40m, first.Quantity);
            },
            second =>
            {
                Assert.Equal(1, second.WarehouseId);
                Assert.Equal(20, second.LocationId);
                Assert.Equal(10m, second.Quantity);
            });
    }

    [Fact]
    public async Task AllocateAsync_WithExactTotalStock_ShouldAllocateAllAvailableStock()
    {
        // Arrange
        SetupAvailableStocks(
        [
            CreateStock(
                locationId: 10,
                locationCode: "A-01-01",
                quantity: 40m),

            CreateStock(
                locationId: 20,
                locationCode: "A-01-02",
                quantity: 25m),

            CreateStock(
                locationId: 30,
                locationCode: "A-01-03",
                quantity: 10m)
        ]);

        var strategy = CreateStrategy();

        // Act
        var allocations = await strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: null,
            quantity: 75m);

        // Assert
        Assert.Equal(3, allocations.Count);
        Assert.Equal(75m, allocations.Sum(allocation => allocation.Quantity));
    }

    [Fact]
    public async Task AllocateAsync_WithInsufficientTotalStock_ShouldThrowConflictException()
    {
        // Arrange
        SetupAvailableStocks(
        [
            CreateStock(
                locationId: 10,
                locationCode: "A-01-01",
                quantity: 40m),

            CreateStock(
                locationId: 20,
                locationCode: "A-01-02",
                quantity: 25m)
        ]);

        var strategy = CreateStrategy();

        // Act
        var act = () => strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: null,
            quantity: 70m);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    [Fact]
    public async Task AllocateAsync_WithNoAvailableStock_ShouldThrowConflictException()
    {
        // Arrange
        SetupAvailableStocks([]);

        var strategy = CreateStrategy();

        // Act
        var act = () => strategy.AllocateAsync(
            productId: 1,
            warehouseId: 1,
            requestedLocationId: null,
            quantity: 10m);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    [Fact]
    public async Task AllocateAsync_ShouldRequestStocksForSpecifiedProductAndWarehouse()
    {
        // Arrange
        SetupAvailableStocks(
        [
            CreateStock(
                locationId: 10,
                locationCode: "A-01-01",
                quantity: 20m)
        ]);

        var strategy = CreateStrategy();

        // Act
        await strategy.AllocateAsync(
            productId: 5,
            warehouseId: 7,
            requestedLocationId: null,
            quantity: 10m);

        // Assert
        _stockReadRepository.Verify(
            repository => repository.GetAvailableStocksAsync(
                5,
                7,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private void SetupAvailableStocks(
        IReadOnlyCollection<StockBalanceModel> stocks)
    {
        _stockReadRepository
            .Setup(repository => repository.GetAvailableStocksAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(stocks);
    }

    private static StockBalanceModel CreateStock(
        int locationId,
        string locationCode,
        decimal quantity)
    {
        return new StockBalanceModel(
            ProductId: 1,
            ProductCode: "PRD-001",
            ProductName: "Product 1",
            WarehouseId: 1,
            WarehouseCode: "WH-001",
            WarehouseName: "Main Warehouse",
            LocationId: locationId,
            LocationCode: locationCode,
            Quantity: quantity);
    }
}