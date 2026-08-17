using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.Common.StockAllocation;
using EvoManage.Application.Inventory.StockMovements.Commands;
using EvoManage.Application.Inventory.StockMovements.Commands.Transfer;
using EvoManage.Application.Inventory.StockMovements.Events;
using EvoManage.Domain.Inventory.StockMovements;
using EvoManage.Domain.Locations;
using EvoManage.Domain.Products;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Commands.Transfer;

public sealed class CreateStockTransferServiceTests
{
    private readonly Mock<IStockMovementRepository> _stockMovementRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly StockAllocationStrategyResolver _stockAllocationStrategyResolver = new([]);
    private readonly StockMovementCreatedEventDispatcher _eventDispatcher = new([]);

    private StockMovementCommandService CreateService()
        => new(
            _stockMovementRepository.Object,
            _productRepository.Object,
            _warehouseRepository.Object,
            _locationRepository.Object,
            _unitOfWork.Object,
            _stockAllocationStrategyResolver,
            _eventDispatcher);

    [Fact]
    public async Task CreateTransferAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var request = new CreateStockTransferRequest(
            ProductId: 999,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 2,
            TargetLocationId: 20,
            Quantity: 5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithMissingSourceWarehouse_ShouldThrowNotFoundException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        var request = new CreateStockTransferRequest(
            ProductId: 1,
            SourceWarehouseId: 999,
            SourceLocationId: 10,
            TargetWarehouseId: 2,
            TargetLocationId: 20,
            Quantity: 5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithMissingSourceLocation_ShouldThrowNotFoundException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var sourceWarehouse = Warehouse.Create(
            "WH-001",
            "Source Warehouse");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceWarehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        var request = new CreateStockTransferRequest(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 999,
            TargetWarehouseId: 2,
            TargetLocationId: 20,
            Quantity: 5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithMissingTargetWarehouse_ShouldThrowNotFoundException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var sourceWarehouse = Warehouse.Create(
            "WH-001",
            "Source Warehouse");

        var sourceLocation = Location.Create(
            warehouseId: 1,
            code: "SOURCE");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceWarehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceLocation);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        var request = new CreateStockTransferRequest(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 999,
            TargetLocationId: 20,
            Quantity: 5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithMissingTargetLocation_ShouldThrowNotFoundException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var sourceWarehouse = Warehouse.Create(
            "WH-001",
            "Source Warehouse");

        var targetWarehouse = Warehouse.Create(
            "WH-002",
            "Target Warehouse");

        var sourceLocation = Location.Create(
            warehouseId: 1,
            code: "SOURCE");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceWarehouse);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetWarehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceLocation);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        var request = new CreateStockTransferRequest(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 2,
            TargetLocationId: 999,
            Quantity: 5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithInactiveProduct_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        product.Deactivate();

        SetupValidTransferRepositories(product);

        var request = CreateValidTransferRequest();
        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithInactiveSourceWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var sourceWarehouse = Warehouse.Create(
            "WH-001",
            "Source Warehouse");

        sourceWarehouse.Deactivate();

        SetupValidTransferRepositories(
            sourceWarehouse: sourceWarehouse);

        var request = CreateValidTransferRequest();
        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithInactiveSourceLocation_ShouldThrowConflictException()
    {
        // Arrange
        var sourceLocation = Location.Create(
            warehouseId: 1,
            code: "SOURCE");

        sourceLocation.Deactivate();

        SetupValidTransferRepositories(
            sourceLocation: sourceLocation);

        var request = CreateValidTransferRequest();
        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithInactiveTargetWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var targetWarehouse = Warehouse.Create(
            "WH-002",
            "Target Warehouse");

        targetWarehouse.Deactivate();

        SetupValidTransferRepositories(
            targetWarehouse: targetWarehouse);

        var request = CreateValidTransferRequest();
        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithInactiveTargetLocation_ShouldThrowConflictException()
    {
        // Arrange
        var targetLocation = Location.Create(
            warehouseId: 2,
            code: "TARGET");

        targetLocation.Deactivate();

        SetupValidTransferRepositories(
            targetLocation: targetLocation);

        var request = CreateValidTransferRequest();
        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    private void SetupValidTransferRepositories(
        Product? product = null,
        Warehouse? sourceWarehouse = null,
        Location? sourceLocation = null,
        Warehouse? targetWarehouse = null,
        Location? targetLocation = null)
    {
        product ??= Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        sourceWarehouse ??= Warehouse.Create(
            "WH-001",
            "Source Warehouse");

        sourceLocation ??= Location.Create(
            warehouseId: 1,
            code: "SOURCE");

        targetWarehouse ??= Warehouse.Create(
            "WH-002",
            "Target Warehouse");

        targetLocation ??= Location.Create(
            warehouseId: 2,
            code: "TARGET");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceWarehouse);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetWarehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceLocation);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetLocation);
    }

    private static CreateStockTransferRequest CreateValidTransferRequest()
        => new(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 2,
            TargetLocationId: 20,
            Quantity: 5m);

    private void VerifyMovementWasNotSaved()
    {
        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<StockMovement>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}