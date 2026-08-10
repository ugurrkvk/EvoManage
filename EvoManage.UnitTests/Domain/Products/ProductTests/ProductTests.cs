using EvoManage.Domain.Common.Exceptions;
using EvoManage.Domain.Products;

namespace EvoManage.UnitTests.Domain.Products;

public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidValues_ShouldCreateActiveProduct()
    {
        // Arrange
        const string code = "PRD-001";
        const string name = "Test Product";
        const ProductTrackingType trackingType = ProductTrackingType.Lot;

        // Act
        var product = Product.Create(code, name, trackingType);

        // Assert
        Assert.Equal(code, product.Code);
        Assert.Equal(name, product.Name);
        Assert.Equal(trackingType, product.TrackingType);
        Assert.True(product.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    public void Create_WithInvalidCode_ShouldThrowDomainException(string code)
    {
        // Arrange
        const string name = "Test Product";
        const ProductTrackingType trackingType = ProductTrackingType.Lot;

        // Act
        var act = () => Product.Create(code, name, trackingType);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    public void Create_WithInvalidName_ShouldThrowDomainException(string name)
    {
        // Arrange
        const string code = "PRD-001";
        const ProductTrackingType trackingType = ProductTrackingType.Lot;

        // Act
        var act = () => Product.Create(code, name, trackingType);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithCodeLongerThan50Characters_ShouldThrowDomainException()
    {
        // Arrange
        var code = new string('A', 51);
        const string name = "Test Product";

        // Act
        var act = () => Product.Create(
            code,
            name,
            ProductTrackingType.Lot);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithInvalidTrackingType_ShouldThrowDomainException()
    {
        // Arrange
        var invalidTrackingType = (ProductTrackingType)999;

        // Act
        var act = () => Product.Create(
            "PRD-001",
            "Test Product",
            invalidTrackingType);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Deactivate_ShouldSetProductAsInactive()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        // Act
        product.Deactivate();

        // Assert
        Assert.False(product.IsActive);
    }

    [Fact]
    public void Activate_AfterDeactivation_ShouldSetProductAsActive()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        product.Deactivate();

        // Act
        product.Activate();

        // Assert
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Create_WithNameLongerThan200Characters_ShouldThrowDomainException()
    {
        // Arrange
        const string code = "PRD-001";
        var name = new string('A', 201);

        // Act
        var act = () => Product.Create(
            code,
            name,
            ProductTrackingType.Lot);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithLeadingAndTrailingSpaces_ShouldTrimValues()
    {
        // Arrange
        const string code = "  PRD-001  ";
        const string name = "  Test Product  ";

        // Act
        var product = Product.Create(
            code,
            name,
            ProductTrackingType.Lot);

        // Assert
        Assert.Equal("PRD-001", product.Code);
        Assert.Equal("Test Product", product.Name);
    }

    [Fact]
    public void Create_WithCodeWithinLimitAfterTrim_ShouldCreateProduct()
    {
        // Arrange
        var validCode = new string('A', 50);
        var code = $"   {validCode}   ";

        // Act
        var product = Product.Create(
            code,
            "Test Product",
            ProductTrackingType.None);

        // Assert
        Assert.Equal(validCode, product.Code);
    }
}