using EvoManage.Domain.Locations;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

public interface IWarehouseLocationValidator
{
    Task<Location> ValidateAsync(int locationId, int warehouseId, CancellationToken cancellationToken = default);
}