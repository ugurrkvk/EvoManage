using EvoManage.Application.Locations.Commands.Update;

namespace EvoManage.UnitTests.Application.Locations.Commands.Update;

public sealed class UpdateLocationRequestValidatorTests
{
    private readonly UpdateLocationRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        var request = new UpdateLocationRequest(
            "1B01K1G002",
            "SP");

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidCode_ShouldFail(string code)
    {
        var request = new UpdateLocationRequest(
            code,
            "SP");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithCodeLongerThan100Characters_ShouldFail()
    {
        var request = new UpdateLocationRequest(
            new string('A', 101),
            "SP");

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithGroupCodeLongerThan50Characters_ShouldFail()
    {
        var request = new UpdateLocationRequest(
            "1B01K1G002",
            new string('A', 51));

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
    }
}