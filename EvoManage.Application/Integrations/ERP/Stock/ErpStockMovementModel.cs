using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.Application.Integrations.ERP.Stock;

public sealed record ErpStockMovementModel(
    int ProductId,
    int WarehouseId,
    int LocationId,
    decimal Quantity,
    StockMovementType MovementType);