using EvoManage.Application.Locations.Commands.Create;

namespace EvoManage.UnitTests.Application.Locations.Commands.Create;

public sealed class CreateLocationRequestValidatorTests
{
    private readonly CreateLocationRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new CreateLocationRequest(
            1,
            "1B01K1G001",
            "YP");

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidWarehouseId_ShouldFail(int warehouseId)
    {
        var request = new CreateLocationRequest(
            warehouseId,
            "1B01K1G001",
            "YP");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidCode_ShouldFail(string code)
    {
        var request = new CreateLocationRequest(
            1,
            code,
            "YP");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithCodeLongerThan100Characters_ShouldFail()
    {
        var request = new CreateLocationRequest(
            1,
            new string('A', 101),
            "YP");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithGroupCodeLongerThan50Characters_ShouldFail()
    {
        var request = new CreateLocationRequest(
            1,
            "1B01K1G001",
            new string('A', 51));

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}