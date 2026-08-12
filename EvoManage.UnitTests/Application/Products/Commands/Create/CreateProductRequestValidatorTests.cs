using EvoManage.Application.Products.Commands.Create;
using EvoManage.Domain.Products;

namespace EvoManage.UnitTests.Application.Products.Commands.Create;

public sealed class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        // Arrange
        var request = new CreateProductRequest(
            "PRD-001",
            "Test Product",
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithEmptyCode_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            "",
            "Test Product",
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateProductRequest.Code));
    }

    [Fact]
    public async Task Validate_WithCodeLongerThan50Characters_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            new string('A', 51),
            "Test Product",
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateProductRequest.Code));
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            "PRD-001",
            "",
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateProductRequest.Name));
    }

    [Fact]
    public async Task Validate_WithNameLongerThan200Characters_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            "PRD-001",
            new string('A', 201),
            ProductTrackingType.Lot);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateProductRequest.Name));
    }

    [Fact]
    public async Task Validate_WithInvalidTrackingType_ShouldFail()
    {
        // Arrange
        var request = new CreateProductRequest(
            "PRD-001",
            "Test Product",
            (ProductTrackingType)999);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateProductRequest.TrackingType));
    }
}