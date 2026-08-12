using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Locations.Commands;
using EvoManage.Application.Locations.Commands.Create;
using EvoManage.Application.Locations.Commands.Update;
using EvoManage.Domain.Locations;
using EvoManage.Domain.Warehouses;
using Moq;

namespace EvoManage.UnitTests.Application.Locations.Commands;

public sealed class LocationCommandServiceTests
{
    private readonly Mock<ILocationRepository> _locationRepository;
    private readonly Mock<IWarehouseRepository> _warehouseRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly LocationCommandService _service;

    public LocationCommandServiceTests()
    {
        _locationRepository = new Mock<ILocationRepository>();
        _warehouseRepository = new Mock<IWarehouseRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _service = new LocationCommandService(
            _locationRepository.Object,
            _warehouseRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldAddLocationAndSaveChanges()
    {
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _locationRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                1,
                "1B01K1G001",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateLocationRequest(
            1,
            "1B01K1G001",
            "YP");

        var response = await _service.CreateAsync(request);

        Assert.NotNull(response);

        _locationRepository.Verify(
            repository => repository.AddAsync(
                It.Is<Location>(location =>
                    location.WarehouseId == 1 &&
                    location.Code == "1B01K1G001" &&
                    location.GroupCode == "YP" &&
                    location.IsActive),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithMissingWarehouse_ShouldThrowNotFoundException()
    {
        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        var request = new CreateLocationRequest(
            999,
            "TEST-001",
            null);

        var act = () => _service.CreateAsync(request);

        await Assert.ThrowsAsync<NotFoundException>(act);

        _locationRepository.Verify(
            repository => repository.ExistsByCodeAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _locationRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Location>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithExistingCode_ShouldThrowConflictException()
    {
        var warehouse = Warehouse.Create(
            "WH-001",
            "Main Warehouse");

        _warehouseRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouse);

        _locationRepository
            .Setup(repository => repository.ExistsByCodeAsync(
                1,
                "1B01K1G001",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateLocationRequest(
            1,
            "1B01K1G001",
            "YP");

        var act = () => _service.CreateAsync(request);

        await Assert.ThrowsAsync<ConflictException>(act);

        _locationRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Location>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldUpdateLocationAndSaveChanges()
    {
        var location = Location.Create(
            1,
            "1B01K1G001",
            "YP");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _locationRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                1,
                "1B01K1G002",
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new UpdateLocationRequest(
            "1B01K1G002",
            "SP");

        await _service.UpdateAsync(
            10,
            request);

        Assert.Equal("1B01K1G002", location.Code);
        Assert.Equal("SP", location.GroupCode);
        Assert.Equal(1, location.WarehouseId);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingCode_ShouldThrowConflictException()
    {
        var location = Location.Create(
            1,
            "1B01K1G001",
            "YP");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _locationRepository
            .Setup(repository => repository.ExistsByCodeExceptIdAsync(
                1,
                "1B01K1G002",
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new UpdateLocationRequest(
            "1B01K1G002",
            "SP");

        var act = () => _service.UpdateAsync(
            10,
            request);

        await Assert.ThrowsAsync<ConflictException>(act);

        Assert.Equal("1B01K1G001", location.Code);
        Assert.Equal("YP", location.GroupCode);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingLocation_ShouldRemoveLocationAndSaveChanges()
    {
        var location = Location.Create(
            1,
            "1B01K1G001");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        await _service.DeleteAsync(10);

        _locationRepository.Verify(
            repository => repository.Remove(location),
            Times.Once);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ActivateAsync_WithExistingLocation_ShouldActivateAndSaveChanges()
    {
        var location = Location.Create(
            1,
            "1B01K1G001");

        location.Deactivate();

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        await _service.ActivateAsync(10);

        Assert.True(location.IsActive);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_WithExistingLocation_ShouldDeactivateAndSaveChanges()
    {
        var location = Location.Create(
            1,
            "1B01K1G001");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        await _service.DeactivateAsync(10);

        Assert.False(location.IsActive);

        _unitOfWork.Verify(
            work => work.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}