using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Products.GetList;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Products.GetList;

public sealed class GetProductListServiceTests
{
    [Fact]
    public async Task GetAsync_WithPagedProducts_ShouldReturnCorrectPagination()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();

        var products = new List<Product>
        {
            Product.Create(
                "PRD-001",
                "Product 1",
                ProductTrackingType.None),

            Product.Create(
                "PRD-002",
                "Product 2",
                ProductTrackingType.Lot)
        };

        productRepository
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        productRepository
            .Setup(repository => repository.CountAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(6);

        var service = new GetProductListService(
            productRepository.Object);

        var request = new GetProductListRequest(
            PageNumber: 1,
            PageSize: 2);

        // Act
        var response = await service.GetAsync(request);

        // Assert
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(2, response.PageSize);
        Assert.Equal(6, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(2, response.Items.Count);
        var firstProduct = response.Items.First();

        Assert.Equal("PRD-001", firstProduct.Code);
        Assert.Equal("Product 1", firstProduct.Name);
        Assert.Equal(ProductTrackingType.None, firstProduct.TrackingType);
        Assert.True(firstProduct.IsActive);
    }
}