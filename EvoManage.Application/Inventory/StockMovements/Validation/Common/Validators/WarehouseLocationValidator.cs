using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Domain.Locations;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

public sealed class WarehouseLocationValidator(ILocationRepository locationRepository) : IWarehouseLocationValidator
{
    public async Task<Location> ValidateAsync(int locationId, int warehouseId, CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null) throw new NotFoundException($"Location with id '{locationId}' was not found.");
        if (!location.IsActive) throw new ConflictException($"Location with id '{locationId}' is not active.");
        if (location.WarehouseId != warehouseId) throw new ConflictException($"Location with id '{locationId}' does not belong to warehouse '{warehouseId}'.");
        return location;
    }
}