using EvoManage.Application.Products.Commands.Update;
using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Products.Commands;
using EvoManage.Application.Products.Commands.Create;
using EvoManage.Domain.Products;
using Moq;

namespace EvoManage.UnitTests.Application.Products.Commands;

public sealed class ProductCommandServiceTests
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ProductCommandService _service;

    public ProductCommandServiceTests()
    {
        _productRepository = new Mock<IProductRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _service = new ProductCommandService(
            _productRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldAddProductAndSaveChanges()
    {
        // Arrange
        var request = new CreateProductRequest(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                request.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(response);

        _productRepository.Verify(
            repository => repository.AddAsync(
                It.Is<Product>(product =>
                    product.Code == "PRD-001" &&
                    product.Name == "Test Product" &&
                    product.TrackingType == ProductTrackingType.None &&
                    product.IsActive),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingCode_ShouldThrowConflictException()
    {
        // Arrange
        var request = new CreateProductRequest(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                request.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        _productRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldUpdateProductAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-003",
            "Old Product",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _productRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                "PRD-003",
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new UpdateProductRequest(
            "PRD-003",
            "Updated Product",
            ProductTrackingType.Lot);

        // Act
        await _service.UpdateAsync(3, request);

        // Assert
        Assert.Equal("PRD-003", product.Code);
        Assert.Equal("Updated Product", product.Name);
        Assert.Equal(ProductTrackingType.Lot, product.TrackingType);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var request = new UpdateProductRequest(
            "PRD-999",
            "Missing Product",
            ProductTrackingType.None);

        // Act
        var act = () => _service.UpdateAsync(999, request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        _productRepository.Verify(
            repository => repository.ExistsByCodeExceptIdAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingCode_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-003",
            "Old Product",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _productRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                "PRD-001",
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new UpdateProductRequest(
            "PRD-001",
            "Updated Product",
            ProductTrackingType.Lot);

        // Act
        var act = () => _service.UpdateAsync(3, request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        Assert.Equal("PRD-003", product.Code);
        Assert.Equal("Old Product", product.Name);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingProduct_ShouldRemoveProductAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _productRepository.Verify(
            repository => repository.Remove(product),
            Times.Once);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = () => _service.DeleteAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        _productRepository.Verify(
            repository => repository.Remove(It.IsAny<Product>()),
            Times.Never);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeactivateAsync_WithExistingProduct_ShouldDeactivateAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        await _service.DeactivateAsync(1);

        // Assert
        Assert.False(product.IsActive);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = () => _service.DeactivateAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ActivateAsync_WithExistingProduct_ShouldActivateAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Test Product",
            ProductTrackingType.None);

        product.Deactivate();

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        await _service.ActivateAsync(1);

        // Assert
        Assert.True(product.IsActive);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = () => _service.ActivateAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}