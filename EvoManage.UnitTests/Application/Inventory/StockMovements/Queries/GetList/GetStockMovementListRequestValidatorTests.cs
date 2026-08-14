using EvoManage.Application.Inventory.StockMovements.Queries.GetList;
using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Queries.GetList;

public sealed class GetStockMovementListRequestValidatorTests
{
    private readonly GetStockMovementListRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new GetStockMovementListRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            MovementType: StockMovementType.Receipt,
            PageNumber: 1,
            PageSize: 20);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithoutOptionalFilters_ShouldSucceed()
    {
        var request = new GetStockMovementListRequest(
            ProductId: null,
            WarehouseId: null,
            LocationId: null,
            MovementType: null,
            PageNumber: 1,
            PageSize: 20);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidProductId_ShouldFail(int productId)
    {
        var request = CreateValidRequest() with
        {
            ProductId = productId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidWarehouseId_ShouldFail(int warehouseId)
    {
        var request = CreateValidRequest() with
        {
            WarehouseId = warehouseId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidLocationId_ShouldFail(int locationId)
    {
        var request = CreateValidRequest() with
        {
            LocationId = locationId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidMovementType_ShouldFail(int movementType)
    {
        var request = CreateValidRequest() with
        {
            MovementType = (StockMovementType)movementType
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidPageNumber_ShouldFail(int pageNumber)
    {
        var request = CreateValidRequest() with
        {
            PageNumber = pageNumber
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Validate_WithInvalidPageSize_ShouldFail(int pageSize)
    {
        var request = CreateValidRequest() with
        {
            PageSize = pageSize
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithMaximumPageSize_ShouldSucceed()
    {
        var request = CreateValidRequest() with
        {
            PageSize = 100
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    private static GetStockMovementListRequest CreateValidRequest()
        => new(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            MovementType: StockMovementType.Receipt,
            PageNumber: 1,
            PageSize: 20);
}