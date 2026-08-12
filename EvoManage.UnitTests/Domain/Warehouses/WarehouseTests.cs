using EvoManage.Domain.Common.Exceptions;
using EvoManage.Domain.Warehouses;

namespace EvoManage.UnitTests.Domain.Warehouses;

public sealed class WarehouseTests
{
    [Fact]
    public void Create_WithValidValues_ShouldCreateActiveWarehouse()
    {
        // Act
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse",
            "Istanbul",
            "Main distribution warehouse");

        // Assert
        Assert.Equal("WH-001", warehouse.Code);
        Assert.Equal("Main Warehouse", warehouse.Name);
        Assert.Equal("Istanbul", warehouse.Address);
        Assert.Equal("Main distribution warehouse", warehouse.Description);
        Assert.True(warehouse.IsActive);
    }

    [Fact]
    public void Create_ShouldTrimValues()
    {
        // Act
        var warehouse = Warehouse.Create(
            "  WH-001  ",
            "  Main Warehouse  ",
            "  Istanbul  ",
            "  Description  ");

        // Assert
        Assert.Equal("WH-001", warehouse.Code);
        Assert.Equal("Main Warehouse", warehouse.Name);
        Assert.Equal("Istanbul", warehouse.Address);
        Assert.Equal("Description", warehouse.Description);
    }

    [Fact]
    public void Create_WithWhitespaceOptionalValues_ShouldNormalizeToNull()
    {
        // Act
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse",
            "   ",
            "   ");

        // Assert
        Assert.Null(warehouse.Address);
        Assert.Null(warehouse.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ShouldThrowDomainException(string code)
    {
        // Act
        var act = () => Warehouse.Create(
            code,
            "Main Warehouse");

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrowDomainException(string name)
    {
        // Act
        var act = () => Warehouse.Create(
            "WH-001",
            name);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithCodeLongerThan50Characters_ShouldThrowDomainException()
    {
        // Arrange
        var code = new string('A', 51);

        // Act
        var act = () => Warehouse.Create(
            code,
            "Main Warehouse");

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithNameLongerThan200Characters_ShouldThrowDomainException()
    {
        // Arrange
        var name = new string('A', 201);

        // Act
        var act = () => Warehouse.Create(
            "WH-001",
            name);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithAddressLongerThan500Characters_ShouldThrowDomainException()
    {
        // Arrange
        var address = new string('A', 501);

        // Act
        var act = () => Warehouse.Create(
            "WH-001",
            "Main Warehouse",
            address);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_WithDescriptionLongerThan1000Characters_ShouldThrowDomainException()
    {
        // Arrange
        var description = new string('A', 1001);

        // Act
        var act = () => Warehouse.Create(
            "WH-001",
            "Main Warehouse",
            description: description);

        // Assert
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Update_WithValidValues_ShouldUpdateWarehouse()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        // Act
        warehouse.Update(
            "WH-002",
            "Updated Warehouse",
            "Ankara",
            "Updated description");

        // Assert
        Assert.Equal("WH-002", warehouse.Code);
        Assert.Equal("Updated Warehouse", warehouse.Name);
        Assert.Equal("Ankara", warehouse.Address);
        Assert.Equal("Updated description", warehouse.Description);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        // Act
        warehouse.Deactivate();

        // Assert
        Assert.False(warehouse.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        warehouse.Deactivate();

        // Act
        warehouse.Activate();

        // Assert
        Assert.True(warehouse.IsActive);
    }
}