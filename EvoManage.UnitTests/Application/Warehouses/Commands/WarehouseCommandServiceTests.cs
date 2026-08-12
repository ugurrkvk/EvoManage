using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Warehouses.Commands;
using EvoManage.Application.Warehouses.Commands.Create;
using EvoManage.Application.Warehouses.Commands.Update;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Warehouses.Commands;

public sealed class WarehouseCommandServiceTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly WarehouseCommandService _service;

    public WarehouseCommandServiceTests()
    {
        _warehouseRepository = new Mock<IWarehouseRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _service = new WarehouseCommandService(
            _warehouseRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldAddWarehouseAndSaveChanges()
    {
        // Arrange
        var request = new CreateWarehouseRequest(
            "WH-001",
            "Main Warehouse",
            "Istanbul",
            "Main distribution warehouse");

        _warehouseRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                request.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(response);

        _warehouseRepository.Verify(
            repository => repository.AddAsync(
                It.Is<Warehouse>(warehouse =>
                    warehouse.Code == request.Code &&
                    warehouse.Name == request.Name &&
                    warehouse.Address == request.Address &&
                    warehouse.Description == request.Description &&
                    warehouse.IsActive),
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
        var request = new CreateWarehouseRequest(
            "WH-001",
            "Main Warehouse",
            null,
            null);

        _warehouseRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                request.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        _warehouseRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Warehouse>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldUpdateWarehouseAndSaveChanges()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Old Warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _warehouseRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                "WH-001",
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new UpdateWarehouseRequest(
            "WH-001",
            "Updated Warehouse",
            "Ankara",
            "Updated description");

        // Act
        await _service.UpdateAsync(1, request);

        // Assert
        Assert.Equal("WH-001", warehouse.Code);
        Assert.Equal("Updated Warehouse", warehouse.Name);
        Assert.Equal("Ankara", warehouse.Address);
        Assert.Equal("Updated description", warehouse.Description);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithMissingWarehouse_ShouldThrowNotFoundException()
    {
        // Arrange
        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        var request = new UpdateWarehouseRequest(
            "WH-999",
            "Missing Warehouse",
            null,
            null);

        // Act
        var act = () => _service.UpdateAsync(999, request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);

        _warehouseRepository.Verify(
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
        var warehouse = Warehouse.Create(
            "WH-001",
            "Old Warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _warehouseRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                "WH-002",
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new UpdateWarehouseRequest(
            "WH-002",
            "Updated Warehouse",
            null,
            null);

        // Act
        var act = () => _service.UpdateAsync(1, request);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);

        Assert.Equal("WH-001", warehouse.Code);
        Assert.Equal("Old Warehouse", warehouse.Name);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingWarehouse_ShouldRemoveWarehouseAndSaveChanges()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _warehouseRepository.Verify(
            repository => repository.Remove(warehouse),
            Times.Once);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_WithExistingWarehouse_ShouldActivateAndSaveChanges()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        warehouse.Deactivate();

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        // Act
        await _service.ActivateAsync(1);

        // Assert
        Assert.True(warehouse.IsActive);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_WithExistingWarehouse_ShouldDeactivateAndSaveChanges()
    {
        // Arrange
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        // Act
        await _service.DeactivateAsync(1);

        // Assert
        Assert.False(warehouse.IsActive);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}