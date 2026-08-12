using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Warehouses.Queries;
using EvoManage.Application.Warehouses.Queries.GetList;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Warehouses.Queries;

public sealed class WarehouseQueryServiceTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository;
    private readonly WarehouseQueryService _service;

    public WarehouseQueryServiceTests()
    {
        _warehouseRepository = new Mock<IWarehouseRepository>();

        _service = new WarehouseQueryService(
            _warehouseRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingWarehouse_ShouldReturnWarehouse()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse",
            "Istanbul",
            "Main distribution warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        // Act
        var response = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(warehouse.Id, response.Id);
        Assert.Equal("WH-001", response.Code);
        Assert.Equal("Main Warehouse", response.Name);
        Assert.Equal("Istanbul", response.Address);
        Assert.Equal(
            "Main distribution warehouse",
            response.Description);
        Assert.True(response.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingWarehouse_ShouldThrowNotFoundException()
    {
        // Arrange
        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        // Act
        var act = () => _service.GetByIdAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetListAsync_WithPagedWarehouses_ShouldReturnCorrectPagination()
    {
        // Arrange
        var warehouses = new List<Warehouse>
        {
            Warehouse.Create(
                "WH-001",
                "Main Warehouse",
                "Istanbul"),

            Warehouse.Create(
                "WH-002",
                "Secondary Warehouse",
                "Ankara")
        };

        _warehouseRepository
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouses);

        _warehouseRepository
            .Setup(repository => repository.CountAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(6);

        var request = new GetWarehouseListRequest(
            PageNumber: 1,
            PageSize: 2);

        // Act
        var response = await _service.GetListAsync(request);

        // Assert
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(2, response.PageSize);
        Assert.Equal(6, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(2, response.Items.Count);

        var firstWarehouse = response.Items.First();

        Assert.Equal("WH-001", firstWarehouse.Code);
        Assert.Equal("Main Warehouse", firstWarehouse.Name);
        Assert.Equal("Istanbul", firstWarehouse.Address);
        Assert.True(firstWarehouse.IsActive);
    }
}