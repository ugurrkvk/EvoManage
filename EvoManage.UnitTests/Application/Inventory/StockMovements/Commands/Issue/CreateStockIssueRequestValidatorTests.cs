using EvoManage.Application.Inventory.Common.StockAllocation;
using EvoManage.Application.Inventory.StockMovements.Commands.Issue;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Commands.Issue;

public sealed class CreateStockIssueRequestValidatorTests
{
    private readonly CreateStockIssueRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidManualLocationRequest_ShouldSucceed()
    {
        var request = new CreateStockIssueRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            Quantity: 5.5m,
            AllocationStrategy: StockAllocationStrategyType.ManualLocation);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithHighestStockAndNoLocation_ShouldSucceed()
    {
        var request = new CreateStockIssueRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: null,
            Quantity: 5.5m,
            AllocationStrategy: StockAllocationStrategyType.HighestStock);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithHighestStockAndLocation_ShouldSucceed()
    {
        var request = new CreateStockIssueRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 10,
            Quantity: 5.5m,
            AllocationStrategy: StockAllocationStrategyType.HighestStock);

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithManualLocationAndNoLocation_ShouldFail()
    {
        var request = new CreateStockIssueRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: null,
            Quantity: 5.5m,
            AllocationStrategy: StockAllocationStrategyType.ManualLocation);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidProductId_ShouldFail(
        int productId)
    {
        var request = CreateValidManualRequest() with
        {
            ProductId = productId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidWarehouseId_ShouldFail(
        int warehouseId)
    {
        var request = CreateValidManualRequest() with
        {
            WarehouseId = warehouseId
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidManualLocationId_ShouldFail(
        int locationId)
    {
        var request = CreateValidManualRequest() with
        {
            LocationId = locationId
        };

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
        var request = CreateValidManualRequest() with
        {
            Quantity = quantity
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithInvalidAllocationStrategy_ShouldFail()
    {
        var request = CreateValidManualRequest() with
        {
            AllocationStrategy = (StockAllocationStrategyType)999
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    private static CreateStockIssueRequest CreateValidManualRequest()
        => new(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 1,
            Quantity: 5m,
            AllocationStrategy: StockAllocationStrategyType.ManualLocation);
}