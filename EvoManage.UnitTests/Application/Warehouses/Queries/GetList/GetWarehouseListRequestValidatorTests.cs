using EvoManage.Application.Warehouses.Queries.GetList;

namespace EvoManage.UnitTests.Application.Warehouses.Queries.GetList;

public sealed class GetWarehouseListRequestValidatorTests
{
    private readonly GetWarehouseListRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new GetWarehouseListRequest(1, 20);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidPageNumber_ShouldFail(int pageNumber)
    {
        var request = new GetWarehouseListRequest(pageNumber, 20);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Validate_WithInvalidPageSize_ShouldFail(int pageSize)
    {
        var request = new GetWarehouseListRequest(1, pageSize);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithMaximumPageSize_ShouldSucceed()
    {
        var request = new GetWarehouseListRequest(1, 100);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}