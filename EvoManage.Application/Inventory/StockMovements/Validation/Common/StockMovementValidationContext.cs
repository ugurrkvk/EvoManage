using EvoManage.Domain.Locations;
using EvoManage.Domain.Products;
using EvoManage.Domain.Warehouses;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Common;

public sealed class StockMovementValidationContext(int productId, int warehouseId, int? locationId, decimal quantity)
{
    public int ProductId { get; } = productId;
    public int WarehouseId { get; } = warehouseId;
    public int? LocationId { get; } = locationId;
    public decimal Quantity { get; } = quantity;

    public Product? Product { get; set; }
    public Warehouse? Warehouse { get; set; }
    public Location? Location { get; set; }
}