using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Locations.Queries.GetById;
using EvoManage.Application.Locations.Queries.GetList;

namespace EvoManage.Application.Locations.Queries;

public sealed class LocationQueryService(
    ILocationRepository locationRepository)
{
    public async Task<GetLocationByIdResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (location is null)
            throw new NotFoundException(
                $"Location with id '{id}' was not found.");

        return new GetLocationByIdResponse(
            location.Id,
            location.WarehouseId,
            location.Code,
            location.GroupCode,
            location.IsActive);
    }

    public async Task<GetLocationListResponse> GetListAsync(
        GetLocationListRequest request,
        CancellationToken cancellationToken = default)
    {
        var locations = await locationRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalCount = await locationRepository.CountAsync(
            cancellationToken);

        var items = locations
            .Select(location => new GetLocationListItemResponse(
                location.Id,
                location.WarehouseId,
                location.Code,
                location.GroupCode,
                location.IsActive))
            .ToArray();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)request.PageSize);

        return new GetLocationListResponse(
            items,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages);
    }
}