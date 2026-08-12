using EvoManage.Domain.Common.Exceptions;
using EvoManage.Domain.Locations;

namespace EvoManage.UnitTests.Domain.Locations;

public sealed class LocationTests
{
    [Fact]
    public void Create_WithValidValues_ShouldCreateActiveLocation()
    {
        // Act
        var location = Location.Create(
            1,
            "1B01K1G001",
            "YP");

        // Assert
        Assert.Equal(1, location.WarehouseId);
        Assert.Equal("1B01K1G001", location.Code);
        Assert.Equal("YP", location.GroupCode);
        Assert.True(location.IsActive);
    }

    [Fact]
    public void Create_ShouldTrimValues()
    {
        // Act
        var location = Location.Create(
            1,
            "  1B01K1G001  ",
            "  YP  ");

        // Assert
        Assert.Equal("1B01K1G001", location.Code);
        Assert.Equal("YP", location.GroupCode);
    }

    [Fact]
    public void Create_WithWhitespaceGroupCode_ShouldNormalizeToNull()
    {
        // Act
        var location = Location.Create(
            1,
            "1B01K1G001",
            "   ");

        // Assert
        Assert.Null(location.GroupCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidWarehouseId_ShouldThrowDomainException(
        int warehouseId)
    {
        // Act
        var act = () => Location.Create(
            warehouseId,
            "1B01K1G001");

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ShouldThrowDomainException(string code)
    {
        // Act
        var act = () => Location.Create(
            1,
            code);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithCodeLongerThan100Characters_ShouldThrowDomainException()
    {
        // Arrange
        var code = new string('A', 101);

        // Act
        var act = () => Location.Create(
            1,
            code);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithGroupCodeLongerThan50Characters_ShouldThrowDomainException()
    {
        // Arrange
        var groupCode = new string('A', 51);

        // Act
        var act = () => Location.Create(
            1,
            "1B01K1G001",
            groupCode);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Update_WithValidValues_ShouldUpdateLocation()
    {
        // Arrange
        var location = Location.Create(
            1,
            "1B01K1G001",
            "YP");

        // Act
        location.Update(
            "1B01K1G002",
            "SP");

        // Assert
        Assert.Equal("1B01K1G002", location.Code);
        Assert.Equal("SP", location.GroupCode);
    }

    [Fact]
    public void Update_ShouldNotChangeWarehouseId()
    {
        // Arrange
        var location = Location.Create(
            1,
            "1B01K1G001");

        // Act
        location.Update(
            "1B01K1G002",
            null);

        // Assert
        Assert.Equal(1, location.WarehouseId);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var location = Location.Create(
            1,
            "1B01K1G001");

        // Act
        location.Deactivate();

        // Assert
        Assert.False(location.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var location = Location.Create(
            1,
            "1B01K1G001");

        location.Deactivate();

        // Act
        location.Activate();

        // Assert
        Assert.True(location.IsActive);
    }
}