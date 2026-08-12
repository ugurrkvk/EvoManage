using EvoManage.Application.Warehouses.Commands.Update;

namespace EvoManage.UnitTests.Application.Warehouses.Commands.Update;

public sealed class UpdateWarehouseRequestValidatorTests
{
    private readonly UpdateWarehouseRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new UpdateWarehouseRequest(
            "WH-001",
            "Main Warehouse",
            "Istanbul",
            "Description");

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidCode_ShouldFail(string code)
    {
        var request = new UpdateWarehouseRequest(
            code,
            "Main Warehouse",
            null,
            null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithCodeLongerThan50Characters_ShouldFail()
    {
        var request = new UpdateWarehouseRequest(
            new string('A', 51),
            "Main Warehouse",
            null,
            null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidName_ShouldFail(string name)
    {
        var request = new UpdateWarehouseRequest(
            "WH-001",
            name,
            null,
            null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithNameLongerThan200Characters_ShouldFail()
    {
        var request = new UpdateWarehouseRequest(
            "WH-001",
            new string('A', 201),
            null,
            null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithAddressLongerThan500Characters_ShouldFail()
    {
        var request = new UpdateWarehouseRequest(
            "WH-001",
            "Main Warehouse",
            new string('A', 501),
            null);

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithDescriptionLongerThan1000Characters_ShouldFail()
    {
        var request = new UpdateWarehouseRequest(
            "WH-001",
            "Main Warehouse",
            null,
            new string('A', 1001));

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}