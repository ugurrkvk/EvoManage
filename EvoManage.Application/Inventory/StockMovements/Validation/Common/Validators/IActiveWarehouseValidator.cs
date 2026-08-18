using EvoManage.Domain.Warehouses;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;

public interface IActiveWarehouseValidator
{
    Task<Warehouse> ValidateAsync(int warehouseId, CancellationToken cancellationToken = default);
}