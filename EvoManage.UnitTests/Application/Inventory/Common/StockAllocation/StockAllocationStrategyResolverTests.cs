using EvoManage.Application.Inventory.Common.StockAllocation;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.Common.StockAllocation;

public sealed class StockAllocationStrategyResolverTests
{
    [Fact]
    public void Resolve_WithRegisteredStrategy_ShouldReturnMatchingStrategy()
    {
        // Arrange
        var manualStrategy = new Mock<IStockAllocationStrategy>();

        manualStrategy
            .SetupGet(strategy => strategy.Type)
            .Returns(StockAllocationStrategyType.ManualLocation);

        var resolver = new StockAllocationStrategyResolver(
        [
            manualStrategy.Object
        ]);

        // Act
        var strategy = resolver.Resolve(
            StockAllocationStrategyType.ManualLocation);

        // Assert
        Assert.Same(
            manualStrategy.Object,
            strategy);
    }

    [Fact]
    public void Resolve_WithUnregisteredStrategy_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var manualStrategy = new Mock<IStockAllocationStrategy>();

        manualStrategy
            .SetupGet(strategy => strategy.Type)
            .Returns(StockAllocationStrategyType.ManualLocation);

        var resolver = new StockAllocationStrategyResolver(
        [
            manualStrategy.Object
        ]);

        // Act
        var act = () => resolver.Resolve(
            StockAllocationStrategyType.HighestStock);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}