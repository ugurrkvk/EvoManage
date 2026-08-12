using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Queries.GetList;
using EvoManage.Application.Products.Queries;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Products.Queries;

public sealed class ProductQueryServiceTests
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly ProductQueryService _service;

    public ProductQueryServiceTests()
    {
        _productRepository = new Mock<IProductRepository>();

        _service = new ProductQueryService(
            _productRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.Lot);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var response = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(product.Id, response.Id);
        Assert.Equal("PRD-001", response.Code);
        Assert.Equal("Test Product", response.Name);
        Assert.Equal(ProductTrackingType.Lot, response.TrackingType);
        Assert.True(response.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = () => _service.GetByIdAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetListAsync_WithPagedProducts_ShouldReturnCorrectPagination()
    {
        // Arrange
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

        _productRepository
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _productRepository
            .Setup(repository => repository.CountAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(6);

        var request = new GetProductListRequest(
            PageNumber: 1,
            PageSize: 2);

        // Act
        var response = await _service.GetListAsync(request);

        // Assert
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(2, response.PageSize);
        Assert.Equal(6, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(2, response.Items.Count);

        var firstProduct = response.Items.First();

        Assert.Equal("PRD-001", firstProduct.Code);
        Assert.Equal("Product 1", firstProduct.Name);
        Assert.Equal(
            ProductTrackingType.None,
            firstProduct.TrackingType);
        Assert.True(firstProduct.IsActive);
    }
}