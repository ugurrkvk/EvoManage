using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Inventory.StockMovements.Commands;
using EvoManage.Application.Inventory.StockMovements.Commands.Issue;
using EvoManage.Application.Inventory.StockMovements.Commands.Receipt;
using EvoManage.Application.Inventory.StockMovements.Commands.Transfer;
using EvoManage.Domain.Inventory.StockMovements;
using EvoManage.Domain.Locations;
using EvoManage.Domain.Products;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Inventory.StockMovements.Commands;

public sealed class StockMovementCommandServiceTests
{
    private readonly Mock<IStockMovementRepository> _stockMovementRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private StockMovementCommandService CreateService()
        => new(
            _stockMovementRepository.Object,
            _productRepository.Object,
            _warehouseRepository.Object,
            _locationRepository.Object,
            _unitOfWork.Object);

    [Fact]
    public async Task CreateReceiptAsync_WithValidRequest_ShouldAddReceiptAndSaveChanges()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        var request = new CreateStockReceiptRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 10,
            Quantity: 25.5m);

        var service = CreateService();

        // Act
        var response = await service.CreateReceiptAsync(request);

        // Assert
        Assert.NotNull(response);

        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<StockMovement>(movement =>
                    movement.ProductId == 1 &&
                    movement.WarehouseId == 1 &&
                    movement.LocationId == 10 &&
                    movement.Quantity == 25.5m &&
                    movement.MovementType == StockMovementType.Receipt),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateReceiptAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var request = new CreateStockReceiptRequest(
            999,
            1,
            10,
            10);

        var service = CreateService();

        // Act
        var act = () => service.CreateReceiptAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateReceiptAsync_WithMissingWarehouse_ShouldThrowNotFoundException()
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

        var request = new CreateStockReceiptRequest(
            1,
            999,
            10,
            10);

        var service = CreateService();

        // Act
        var act = () => service.CreateReceiptAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateReceiptAsync_WithMissingLocation_ShouldThrowNotFoundException()
    {
        // Arrange
        SetupValidProductAndWarehouse();

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        var request = new CreateStockReceiptRequest(
            1,
            1,
            999,
            10);

        var service = CreateService();

        // Act
        var act = () => service.CreateReceiptAsync(request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateReceiptAsync_WithInactiveProduct_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        product.Deactivate();

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            1,
            "A-01-01");

        SetupRepositories(product, warehouse, location);

        var request = new CreateStockReceiptRequest(
            1,
            1,
            10,
            10);

        var service = CreateService();

        // Act
        var act = () => service.CreateReceiptAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateReceiptAsync_WithInactiveWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        warehouse.Deactivate();

        var location = Location.Create(
            1,
            "A-01-01");

        SetupRepositories(product, warehouse, location);

        var request = new CreateStockReceiptRequest(
            1,
            1,
            10,
            10);

        var service = CreateService();

        // Act
        var act = () => service.CreateReceiptAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateReceiptAsync_WithInactiveLocation_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            1,
            "A-01-01");

        location.Deactivate();

        SetupRepositories(product, warehouse, location);

        var request = new CreateStockReceiptRequest(
            1,
            1,
            10,
            10);

        var service = CreateService();

        // Act
        var act = () => service.CreateReceiptAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateReceiptAsync_WithLocationBelongingToDifferentWarehouse_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            warehouseId: 2,
            code: "A-01-01");

        SetupRepositories(product, warehouse, location);

        var request = new CreateStockReceiptRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 10,
            Quantity: 10);

        var service = CreateService();

        // Act
        var act = () => service.CreateReceiptAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateIssueAsync_WithSufficientStock_ShouldAddIssueAndSaveChanges()
    {
        // Arrange
        SetupValidMasterData();

        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(25.5m);

        var request = new CreateStockIssueRequest(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 10,
            Quantity: 5m);

        var service = CreateService();

        // Act
        var response = await service.CreateIssueAsync(request);

        // Assert
        Assert.NotNull(response);

        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<StockMovement>(movement =>
                    movement.ProductId == 1 &&
                    movement.WarehouseId == 1 &&
                    movement.LocationId == 10 &&
                    movement.Quantity == 5m &&
                    movement.MovementType == StockMovementType.Issue),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateIssueAsync_WithExactStock_ShouldAddIssueAndSaveChanges()
    {
        // Arrange
        SetupValidMasterData();

        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(25.5m);

        var request = new CreateStockIssueRequest(
            1,
            1,
            10,
            25.5m);

        var service = CreateService();

        // Act
        await service.CreateIssueAsync(request);

        // Assert
        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<StockMovement>(movement =>
                    movement.Quantity == 25.5m &&
                    movement.MovementType == StockMovementType.Issue),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateIssueAsync_WithInsufficientStock_ShouldThrowConflictException()
    {
        // Arrange
        SetupValidMasterData();

        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(25.5m);

        var request = new CreateStockIssueRequest(
            1,
            1,
            10,
            30m);

        var service = CreateService();

        // Act
        var act = () => service.CreateIssueAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateIssueAsync_WithZeroStock_ShouldThrowConflictException()
    {
        // Arrange
        SetupValidMasterData();

        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var request = new CreateStockIssueRequest(
            1,
            1,
            10,
            5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateIssueAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateIssueAsync_WithMissingProduct_ShouldThrowNotFoundException()
    {
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var request = new CreateStockIssueRequest(
            999,
            1,
            10,
            5m);

        var service = CreateService();

        var act = () => service.CreateIssueAsync(request);

        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateIssueAsync_WithMissingWarehouse_ShouldThrowNotFoundException()
    {
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

        var request = new CreateStockIssueRequest(
            1,
            999,
            10,
            5m);

        var service = CreateService();

        var act = () => service.CreateIssueAsync(request);

        await Assert.ThrowsAsync<NotFoundException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateIssueAsync_WithLocationBelongingToDifferentWarehouse_ShouldThrowConflictException()
    {
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            warehouseId: 2,
            code: "A-01-01");

        SetupRepositories(product, warehouse, location);

        var request = new CreateStockIssueRequest(
            1,
            1,
            10,
            5m);

        var service = CreateService();

        var act = () => service.CreateIssueAsync(request);

        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithSufficientStock_ShouldAddTransferOutAndTransferInAndSaveOnce()
    {
        // Arrange
        SetupValidTransferMasterData();

        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(30.5m);

        var request = new CreateStockTransferRequest(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 2,
            TargetLocationId: 20,
            Quantity: 10m);

        var service = CreateService();

        // Act
        var response = await service.CreateTransferAsync(request);

        // Assert
        Assert.NotNull(response);

        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<StockMovement>(movement =>
                    movement.ProductId == 1 &&
                    movement.WarehouseId == 1 &&
                    movement.LocationId == 10 &&
                    movement.Quantity == 10m &&
                    movement.MovementType == StockMovementType.TransferOut),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<StockMovement>(movement =>
                    movement.ProductId == 1 &&
                    movement.WarehouseId == 2 &&
                    movement.LocationId == 20 &&
                    movement.Quantity == 10m &&
                    movement.MovementType == StockMovementType.TransferIn),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<StockMovement>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateTransferAsync_WithExactStock_ShouldSucceed()
    {
        // Arrange
        SetupValidTransferMasterData();

        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(30.5m);

        var request = new CreateStockTransferRequest(
            1,
            1,
            10,
            2,
            20,
            30.5m);

        var service = CreateService();

        // Act
        await service.CreateTransferAsync(request);

        // Assert
        _stockMovementRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<StockMovement>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateTransferAsync_WithInsufficientStock_ShouldThrowConflictException()
    {
        // Arrange
        SetupValidTransferMasterData();

        _stockMovementRepository
            .Setup(repository => repository.GetStockAsync(
                1,
                1,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(30.5m);

        var request = new CreateStockTransferRequest(
            1,
            1,
            10,
            2,
            20,
            40m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithSameSourceAndTarget_ShouldThrowConflictException()
    {
        // Arrange
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        var request = new CreateStockTransferRequest(
            ProductId: 1,
            SourceWarehouseId: 1,
            SourceLocationId: 10,
            TargetWarehouseId: 1,
            TargetLocationId: 10,
            Quantity: 5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithSourceLocationBelongingToDifferentWarehouse_ShouldThrowConflictException()
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
            warehouseId: 999,
            code: "SOURCE");

        var targetLocation = Location.Create(
            warehouseId: 2,
            code: "TARGET");

        SetupTransferRepositories(
            product,
            sourceWarehouse,
            sourceLocation,
            targetWarehouse,
            targetLocation);

        var request = new CreateStockTransferRequest(
            1,
            1,
            10,
            2,
            20,
            5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    [Fact]
    public async Task CreateTransferAsync_WithTargetLocationBelongingToDifferentWarehouse_ShouldThrowConflictException()
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

        var targetLocation = Location.Create(
            warehouseId: 999,
            code: "TARGET");

        SetupTransferRepositories(
            product,
            sourceWarehouse,
            sourceLocation,
            targetWarehouse,
            targetLocation);

        var request = new CreateStockTransferRequest(
            1,
            1,
            10,
            2,
            20,
            5m);

        var service = CreateService();

        // Act
        var act = () => service.CreateTransferAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        VerifyMovementWasNotSaved();
    }

    private void SetupValidTransferMasterData()
    {
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

        var targetLocation = Location.Create(
            warehouseId: 2,
            code: "TARGET");

        SetupTransferRepositories(
            product,
            sourceWarehouse,
            sourceLocation,
            targetWarehouse,
            targetLocation);
    }

    private void SetupTransferRepositories(
        Product product,
        Warehouse sourceWarehouse,
        Location sourceLocation,
        Warehouse targetWarehouse,
        Location targetLocation)
    {
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

    private void SetupValidMasterData()
    {
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        var location = Location.Create(
            warehouseId: 1,
            code: "A-01-01");

        SetupRepositories(
            product,
            warehouse,
            location);
    }

    private void SetupValidProductAndWarehouse()
    {
        var product = Product.Create(
            "PRD-001",
            "Product 1",
            ProductTrackingType.None);

        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);
    }

    private void SetupRepositories(
        Product product,
        Warehouse warehouse,
        Location location)
    {
        _productRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
    }

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