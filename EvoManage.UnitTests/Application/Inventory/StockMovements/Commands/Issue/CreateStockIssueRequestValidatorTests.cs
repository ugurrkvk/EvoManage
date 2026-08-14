using EvoManage.Application.Inventory.StockMovements.Commands.Issue;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Commands.Issue;

public sealed class CreateStockIssueRequestValidatorTests
{
    private readonly CreateStockIssueRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new CreateStockIssueRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            Quantity: 5.5m);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidProductId_ShouldFail(
        int productId)
    {
        var request = new CreateStockIssueRequest(
            productId,
            1,
            1,
            5);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidWarehouseId_ShouldFail(
        int warehouseId)
    {
        var request = new CreateStockIssueRequest(
            1,
            warehouseId,
            1,
            5);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidLocationId_ShouldFail(
        int locationId)
    {
        var request = new CreateStockIssueRequest(
            1,
            1,
            locationId,
            5);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5.5)]
    public async Task Validate_WithInvalidQuantity_ShouldFail(
        decimal quantity)
    {
        var request = new CreateStockIssueRequest(
            1,
            1,
            1,
            quantity);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}