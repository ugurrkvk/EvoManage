using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Locations.Commands.Create;
using EvoManage.Application.Locations.Commands.Update;
using EvoManage.Domain.Locations;

namespace EvoManage.Application.Locations.Commands;

public sealed class LocationCommandService(
    ILocationRepository locationRepository,
    IWarehouseRepository warehouseRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateLocationResponse> CreateAsync(
        CreateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(
            request.WarehouseId,
            cancellationToken);

        if (warehouse is null)
            throw new NotFoundException(
                $"Warehouse with id '{request.WarehouseId}' was not found.");

        var exists = await locationRepository.ExistsByCodeAsync(
            request.WarehouseId,
            request.Code,
            cancellationToken);

        if (exists)
            throw new ConflictException(
                $"Location code '{request.Code}' already exists in warehouse '{request.WarehouseId}'.");

        var location = Location.Create(
            request.WarehouseId,
            request.Code,
            request.GroupCode);

        await locationRepository.AddAsync(
            location,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateLocationResponse(location.Id);
    }

    public async Task UpdateAsync(
        int id,
        UpdateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var location = await GetLocationOrThrowAsync(
            id,
            cancellationToken);

        var exists = await locationRepository.ExistsByCodeExceptIdAsync(
            location.WarehouseId,
            request.Code,
            id,
            cancellationToken);

        if (exists)
            throw new ConflictException(
                $"Location code '{request.Code}' already exists in warehouse '{location.WarehouseId}'.");

        location.Update(
            request.Code,
            request.GroupCode);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var location = await GetLocationOrThrowAsync(
            id,
            cancellationToken);

        locationRepository.Remove(location);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var location = await GetLocationOrThrowAsync(
            id,
            cancellationToken);

        location.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var location = await GetLocationOrThrowAsync(
            id,
            cancellationToken);

        location.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Location> GetLocationOrThrowAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await locationRepository.GetByIdAsync(
                   id,
                   cancellationToken)
               ?? throw new NotFoundException(
                   $"Location with id '{id}' was not found.");
    }
}