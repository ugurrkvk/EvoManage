using EvoManage.Application.Products.GetList;

namespace EvoManage.UnitTests.Application.Products.GetList;

public sealed class GetProductListRequestValidatorTests
{
    private readonly GetProductListRequestValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidRequest_ShouldSucceed()
    {
        // Arrange
        var request = new GetProductListRequest(
            PageNumber: 1,
            PageSize: 20);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidPageNumber_ShouldFail(
        int pageNumber)
    {
        // Arrange
        var request = new GetProductListRequest(
            PageNumber: pageNumber,
            PageSize: 20);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName ==
                     nameof(GetProductListRequest.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(500)]
    public async Task Validate_WithInvalidPageSize_ShouldFail(
        int pageSize)
    {
        // Arrange
        var request = new GetProductListRequest(
            PageNumber: 1,
            PageSize: pageSize);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName ==
                     nameof(GetProductListRequest.PageSize));
    }

    [Fact]
    public async Task Validate_WithMaximumPageSize_ShouldSucceed()
    {
        // Arrange
        var request = new GetProductListRequest(
            PageNumber: 1,
            PageSize: 100);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
}