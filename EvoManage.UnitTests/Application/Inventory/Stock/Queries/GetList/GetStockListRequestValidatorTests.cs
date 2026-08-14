using EvoManage.Application.Inventory.Stock.Queries.GetList;

namespace EvoManage.UnitTests.Application.Inventory.Stock.Queries.GetList;

public sealed class GetStockListRequestValidatorTests
{
    private readonly GetStockListRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new GetStockListRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            IncludeZeroStock: false,
            PageNumber: 1,
            PageSize: 20);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithoutOptionalFilters_ShouldSucceed()
    {
        var request = new GetStockListRequest(
            ProductId: null,
            WarehouseId: null,
            LocationId: null,
            IncludeZeroStock: false,
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

    [Fact]
    public async Task Validate_WithIncludeZeroStock_ShouldSucceed()
    {
        var request = CreateValidRequest() with
        {
            IncludeZeroStock = true
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    private static GetStockListRequest CreateValidRequest()
        => new(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            IncludeZeroStock: false,
            PageNumber: 1,
            PageSize: 20);
}