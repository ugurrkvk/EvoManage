using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Locations.Queries;
using EvoManage.Application.Locations.Queries.GetList;
using EvoManage.Domain.Locations;
using Moq;

namespace EvoManage.UnitTests.Application.Locations.Queries;

public sealed class LocationQueryServiceTests
{
    private readonly Mock<ILocationRepository> _locationRepository;
    private readonly LocationQueryService _service;

    public LocationQueryServiceTests()
    {
        _locationRepository = new Mock<ILocationRepository>();

        _service = new LocationQueryService(
            _locationRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingLocation_ShouldReturnLocation()
    {
        // Arrange
        var location = Location.Create(
            1,
            "1B01K1G001",
            "YP");

        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        // Act
        var response = await _service.GetByIdAsync(10);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(location.Id, response.Id);
        Assert.Equal(1, response.WarehouseId);
        Assert.Equal("1B01K1G001", response.Code);
        Assert.Equal("YP", response.GroupCode);
        Assert.True(response.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingLocation_ShouldThrowNotFoundException()
    {
        // Arrange
        _locationRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        // Act
        var act = () => _service.GetByIdAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetListAsync_WithPagedLocations_ShouldReturnCorrectPagination()
    {
        // Arrange
        var locations = new List<Location>
        {
            Location.Create(
                1,
                "1B01K1G001",
                "YP"),

            Location.Create(
                1,
                "1B01K1G002",
                "YP")
        };

        _locationRepository
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        _locationRepository
            .Setup(repository => repository.CountAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(6);

        var request = new GetLocationListRequest(
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

        var firstLocation = response.Items.First();

        Assert.Equal(1, firstLocation.WarehouseId);
        Assert.Equal("1B01K1G001", firstLocation.Code);
        Assert.Equal("YP", firstLocation.GroupCode);
        Assert.True(firstLocation.IsActive);
    }
}