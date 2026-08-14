using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.Stock.Queries;
using EvoManage.Application.Inventory.Stock.Queries.GetBalance;
using EvoManage.Application.Inventory.Stock.Queries.GetList;
using EvoManage.Application.Inventory.Stocks.Models;
using EvoManage.Domain.Locations;
using EvoManage.Domain.Products;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.Stock.Queries;

public sealed class StockQueryServiceTests
{
    private readonly Mock<IStockReadRepository> _stockReadRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();

    private StockQueryService CreateService()
        => new(
            _stockReadRepository.Object,
            _productRepository.Object,
            _warehouseRepository.Object,
            _locationRepository.Object);

    [Fact]
    public async Task GetBalanceAsync_WithValidRequest_ShouldReturnStockBalance()
    {
        // Arrange
        SetupValidMasterData();

        _stockReadRepository
            .Setup(repository => repository.GetBalanceAsync(
                1,
                1,
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(25.5m);

        var request = new GetStockBalanceRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1);

        var service = CreateService();

        // Act
        var response = await service.GetBalanceAsync(request);

        // Assert
        Assert.Equal(1, response.ProductId);
        Assert.Equal(1, response.WarehouseId);
        Assert.Equal(1, response.LocationId);
        Assert.Equal(25.5m, response.Quantity);
    }

    [Fact]
    public async Task GetBalanceAsync_WithNoMovements_ShouldReturnZero()
    {
        // Arrange
        SetupValidMasterData();

        _stockReadRepository
            .Setup(repository => repository.GetBalanceAsync(
                1,
                1,
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var request = new GetStockBalanceRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1);

        var service = CreateService();

        // Act
        var response = await service.GetBalanceAsync(request);

        // Assert
        Assert.Equal(0m, response.Quantity);
    }

    [Fact]
    public async Task GetBalanceAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var request = new GetStockBalanceRequest(
            999,
            1,
            1);

        var service = CreateService();

        // Act
        var act = () => service.GetBalanceAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task GetBalanceAsync_WithMissingWarehouse_ShouldThrowNotFoundException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        var request = new GetStockBalanceRequest(
            1,
            999,
            1);

        var service = CreateService();

        // Act
        var act = () => service.GetBalanceAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task GetBalanceAsync_WithMissingLocation_ShouldThrowNotFoundException()
    {
        // Arrange
        SetupValidProductAndWarehouse();

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        var request = new GetStockBalanceRequest(
            1,
            1,
            999);

        var service = CreateService();

        // Act
        var act = () => service.GetBalanceAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task GetBalanceAsync_WithLocationBelongingToDifferentWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            warehouseId: 2,
            code: "A-01-01");

        SetupRepositories(
            product,
            warehouse,
            location);

        var request = new GetStockBalanceRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1);

        var service = CreateService();

        // Act
        var act = () => service.GetBalanceAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyStockWasNotQueried();
    }

    [Fact]
    public async Task GetListAsync_WithStocks_ShouldReturnCorrectPaginationAndItems()
    {
        // Arrange
        var stocks = new List<StockBalanceModel>
    {
        new(
            ProductId: 1,
            ProductCode: "PRD-001",
            ProductName: "Product 1",
            WarehouseId: 1,
            WarehouseCode: "WH-001",
            WarehouseName: "Main Warehouse",
            LocationId: 1,
            LocationCode: "A-01-01",
            Quantity: 0.5m),

        new(
            ProductId: 1,
            ProductCode: "PRD-001",
            ProductName: "Product 1",
            WarehouseId: 3,
            WarehouseCode: "WH-003",
            WarehouseName: "Target Warehouse",
            LocationId: 4,
            LocationCode: "B-01-01",
            Quantity: 100m)
    };

        var request = new GetStockListRequest(
            ProductId: 1,
            WarehouseId: null,
            LocationId: null,
            IncludeZeroStock: false,
            PageNumber: 1,
            PageSize: 20);

        _stockReadRepository
            .Setup(repository => repository.GetPagedAsync(
                1,
                null,
                null,
                false,
                1,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(stocks);

        _stockReadRepository
            .Setup(repository => repository.CountAsync(
                1,
                null,
                null,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var service = CreateService();

        // Act
        var response = await service.GetListAsync(request);

        // Assert
        Assert.Equal(2, response.Items.Count);
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(20, response.PageSize);
        Assert.Equal(2, response.TotalCount);
        Assert.Equal(1, response.TotalPages);

        var firstStock = response.Items.First();

        Assert.Equal(1, firstStock.ProductId);
        Assert.Equal("PRD-001", firstStock.ProductCode);
        Assert.Equal("Product 1", firstStock.ProductName);
        Assert.Equal(1, firstStock.WarehouseId);
        Assert.Equal("WH-001", firstStock.WarehouseCode);
        Assert.Equal(1, firstStock.LocationId);
        Assert.Equal("A-01-01", firstStock.LocationCode);
        Assert.Equal(0.5m, firstStock.Quantity);
    }

    [Fact]
    public async Task GetListAsync_WithFilters_ShouldPassFiltersToRepository()
    {
        // Arrange
        var request = new GetStockListRequest(
            ProductId: 1,
            WarehouseId: 3,
            LocationId: 4,
            IncludeZeroStock: false,
            PageNumber: 2,
            PageSize: 10);

        _stockReadRepository
            .Setup(repository => repository.GetPagedAsync(
                1,
                3,
                4,
                false,
                2,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _stockReadRepository
            .Setup(repository => repository.CountAsync(
                1,
                3,
                4,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var service = CreateService();

        // Act
        await service.GetListAsync(request);

        // Assert
        _stockReadRepository.Verify(
            repository => repository.GetPagedAsync(
                1,
                3,
                4,
                false,
                2,
                10,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetListAsync_WithIncludeZeroStock_ShouldPassTrueToRepository()
    {
        // Arrange
        var request = new GetStockListRequest(
            ProductId: null,
            WarehouseId: null,
            LocationId: null,
            IncludeZeroStock: true,
            PageNumber: 1,
            PageSize: 20);

        _stockReadRepository
            .Setup(repository => repository.GetPagedAsync(
                null,
                null,
                null,
                true,
                1,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _stockReadRepository
            .Setup(repository => repository.CountAsync(
                null,
                null,
                null,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var service = CreateService();

        // Act
        var response = await service.GetListAsync(request);

        // Assert
        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalCount);
        Assert.Equal(0, response.TotalPages);
    }

    private void SetupValidMasterData()
    {
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        SetupRepositories(
            product,
            warehouse,
            location);
    }

    private void SetupValidProductAndWarehouse()
    {
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
    }

    private void SetupRepositories(
        Product product,
        Warehouse warehouse,
        Location location)
    {
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
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