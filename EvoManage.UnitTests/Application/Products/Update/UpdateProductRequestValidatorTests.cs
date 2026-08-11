using EvoManage.Application.Products.Update;
using EvoManage.Domain.Products;

namespace EvoManage.UnitTests.Application.Products.Update;

public sealed class UpdateProductRequestValidatorTests
{
    private readonly UpdateProductRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        // Arrange
        var request = new UpdateProductRequest(
            "PRD-001",
            "Updated Product",
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidCode_ShouldFail(string code)
    {
        // Arrange
        var request = new UpdateProductRequest(
            code,
            "Updated Product",
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(UpdateProductRequest.Code));
    }

    [Fact]
    public async Task Validate_WithCodeLongerThan50Characters_ShouldFail()
    {
        // Arrange
        var request = new UpdateProductRequest(
            new string('A', 51),
            "Updated Product",
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(UpdateProductRequest.Code));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithInvalidName_ShouldFail(string name)
    {
        // Arrange
        var request = new UpdateProductRequest(
            "PRD-001",
            name,
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(UpdateProductRequest.Name));
    }

    [Fact]
    public async Task Validate_WithNameLongerThan200Characters_ShouldFail()
    {
        // Arrange
        var request = new UpdateProductRequest(
            "PRD-001",
            new string('A', 201),
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(UpdateProductRequest.Name));
    }

    [Fact]
    public async Task Validate_WithInvalidTrackingType_ShouldFail()
    {
        // Arrange
        var request = new UpdateProductRequest(
            "PRD-001",
            "Updated Product",
            (ProductTrackingType)999);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName ==
                     nameof(UpdateProductRequest.TrackingType));
    }
}