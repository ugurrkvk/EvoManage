using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Inventory.StockMovements.Queries;
using EvoManage.Application.Inventory.StockMovements.Queries.GetList;
using EvoManage.Domain.Inventory.StockMovements;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Queries;

public sealed class StockMovementQueryServiceTests
{
    private readonly Mock<IStockMovementRepository> _stockMovementRepository = new();
    private readonly StockMovementQueryService _service;

    public StockMovementQueryServiceTests()
    {
        _service = new StockMovementQueryService(
            _stockMovementRepository.Object);
    }

    [Fact]
    public async Task GetListAsync_WithFilters_ShouldReturnCorrectPaginationAndItems()
    {
        // Arrange
        var movements = new List<StockMovement>
        {
            StockMovement.Create(
                productId: 1,
                warehouseId: 1,
                locationId: 10,
                quantity: 25.5m,
                StockMovementType.Receipt),

            StockMovement.Create(
                productId: 1,
                warehouseId: 1,
                locationId: 10,
                quantity: 5m,
                StockMovementType.Issue)
        };

        var request = new GetStockMovementListRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 10,
            MovementType: null,
            PageNumber: 1,
            PageSize: 2);

        _stockMovementRepository
            .Setup(repository => repository.GetPagedAsync(
                request.ProductId,
                request.WarehouseId,
                request.LocationId,
                request.MovementType,
                request.PageNumber,
                request.PageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(movements);

        _stockMovementRepository
            .Setup(repository => repository.CountAsync(
                request.ProductId,
                request.WarehouseId,
                request.LocationId,
                request.MovementType,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(6);

        // Act
        var response = await _service.GetListAsync(request);

        // Assert
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(2, response.PageSize);
        Assert.Equal(6, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(2, response.Items.Count);

        var firstMovement = response.Items.First();

        Assert.Equal(1, firstMovement.ProductId);
        Assert.Equal(1, firstMovement.WarehouseId);
        Assert.Equal(10, firstMovement.LocationId);
        Assert.Equal(25.5m, firstMovement.Quantity);
        Assert.Equal(
            StockMovementType.Receipt,
            firstMovement.MovementType);
    }

    [Fact]
    public async Task GetListAsync_WithoutFilters_ShouldPassNullFiltersToRepository()
    {
        // Arrange
        var request = new GetStockMovementListRequest(
            ProductId: null,
            WarehouseId: null,
            LocationId: null,
            MovementType: null,
            PageNumber: 1,
            PageSize: 20);

        _stockMovementRepository
            .Setup(repository => repository.GetPagedAsync(
                null,
                null,
                null,
                null,
                1,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _stockMovementRepository
            .Setup(repository => repository.CountAsync(
                null,
                null,
                null,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var response = await _service.GetListAsync(request);

        // Assert
        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalCount);
        Assert.Equal(0, response.TotalPages);

        _stockMovementRepository.Verify(
            repository => repository.GetPagedAsync(
                null,
                null,
                null,
                null,
                1,
                20,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetListAsync_WithMovementTypeFilter_ShouldPassFilterToRepository()
    {
        // Arrange
        var request = new GetStockMovementListRequest(
            ProductId: 1,
            WarehouseId: null,
            LocationId: null,
            MovementType: StockMovementType.TransferOut,
            PageNumber: 1,
            PageSize: 20);

        _stockMovementRepository
            .Setup(repository => repository.GetPagedAsync(
                1,
                null,
                null,
                StockMovementType.TransferOut,
                1,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _stockMovementRepository
            .Setup(repository => repository.CountAsync(
                1,
                null,
                null,
                StockMovementType.TransferOut,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _service.GetListAsync(request);

        // Assert
        _stockMovementRepository.Verify(
            repository => repository.GetPagedAsync(
                1,
                null,
                null,
                StockMovementType.TransferOut,
                1,
                20,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}