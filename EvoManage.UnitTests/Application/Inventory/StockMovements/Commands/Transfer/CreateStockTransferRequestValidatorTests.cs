using EvoManage.Application.Inventory.StockMovements.Commands.Transfer;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Commands.Transfer;

public sealed class CreateStockTransferRequestValidatorTests
{
    private readonly CreateStockTransferRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new CreateStockTransferRequest(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 2,
            TargetLocationId: 20,
            Quantity: 10m);

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
    public async Task Validate_WithInvalidSourceWarehouseId_ShouldFail(
        int sourceWarehouseId)
    {
        var request = CreateValidRequest() with
        {
            SourceWarehouseId = sourceWarehouseId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidSourceLocationId_ShouldFail(
        int sourceLocationId)
    {
        var request = CreateValidRequest() with
        {
            SourceLocationId = sourceLocationId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidTargetWarehouseId_ShouldFail(
        int targetWarehouseId)
    {
        var request = CreateValidRequest() with
        {
            TargetWarehouseId = targetWarehouseId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidTargetLocationId_ShouldFail(
        int targetLocationId)
    {
        var request = CreateValidRequest() with
        {
            TargetLocationId = targetLocationId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public async Task Validate_WithInvalidQuantity_ShouldFail(decimal quantity)
    {
        var request = CreateValidRequest() with
        {
            Quantity = quantity
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    private static CreateStockTransferRequest CreateValidRequest()
        => new(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 2,
            TargetLocationId: 20,
            Quantity: 10m);
}