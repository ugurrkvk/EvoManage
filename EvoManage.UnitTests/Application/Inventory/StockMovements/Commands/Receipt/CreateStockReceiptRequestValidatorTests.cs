using EvoManage.Application.Inventory.StockMovements.Commands.Receipt;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Commands.Receipt;

public sealed class CreateStockReceiptRequestValidatorTests
{
    private readonly CreateStockReceiptRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new CreateStockReceiptRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            Quantity: 10.5m);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidProductId_ShouldFail(int productId)
    {
        var request = new CreateStockReceiptRequest(
            productId,
            1,
            1,
            10);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidWarehouseId_ShouldFail(int warehouseId)
    {
        var request = new CreateStockReceiptRequest(
            1,
            warehouseId,
            1,
            10);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidLocationId_ShouldFail(int locationId)
    {
        var request = new CreateStockReceiptRequest(
            1,
            1,
            locationId,
            10);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public async Task Validate_WithInvalidQuantity_ShouldFail(decimal quantity)
    {
        var request = new CreateStockReceiptRequest(
            1,
            1,
            1,
            quantity);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}