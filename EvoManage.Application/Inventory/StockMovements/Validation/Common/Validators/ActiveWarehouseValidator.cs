using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Domain.Warehouses;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

public sealed class ActiveWarehouseValidator(IWarehouseRepository warehouseRepository) : IActiveWarehouseValidator
{
    public async Task<Warehouse> ValidateAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null) throw new NotFoundException($"Warehouse with id '{warehouseId}' was not found.");
        if (!warehouse.IsActive) throw new ConflictException($"Warehouse with id '{warehouseId}' is not active.");
        return warehouse;
    }
}