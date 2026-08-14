using EvoManage.Application.Inventory.Stock.Queries.GetBalance;

namespace EvoManage.UnitTests.Application.Inventory.Stock.Queries.GetBalance;

public sealed class GetStockBalanceRequestValidatorTests
{
    private readonly GetStockBalanceRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new GetStockBalanceRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidProductId_ShouldFail(
        int productId)
    {
        var request = new GetStockBalanceRequest(
            productId,
            1,
            1);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidWarehouseId_ShouldFail(
        int warehouseId)
    {
        var request = new GetStockBalanceRequest(
            1,
            warehouseId,
            1);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidLocationId_ShouldFail(
        int locationId)
    {
        var request = new GetStockBalanceRequest(
            1,
            1,
            locationId);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}