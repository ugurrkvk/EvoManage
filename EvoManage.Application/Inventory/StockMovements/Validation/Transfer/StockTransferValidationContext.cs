using EvoManage.Domain.Locations;
using EvoManage.Domain.Products;
using EvoManage.Domain.Warehouses;

namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer;

public sealed class StockTransferValidationContext(int productId, int sourceWarehouseId, int sourceLocationId, int targetWarehouseId, int targetLocationId, decimal quantity)
{
    public int ProductId { get; } = productId;
    public int SourceWarehouseId { get; } = sourceWarehouseId;
    public int SourceLocationId { get; } = sourceLocationId;
    public int TargetWarehouseId { get; } = targetWarehouseId;
    public int TargetLocationId { get; } = targetLocationId;
    public decimal Quantity { get; } = quantity;

    public Product? Product { get; set; }
    public Warehouse? SourceWarehouse { get; set; }
    public Location? SourceLocation { get; set; }
    public Warehouse? TargetWarehouse { get; set; }
    public Location? TargetLocation { get; set; }
}