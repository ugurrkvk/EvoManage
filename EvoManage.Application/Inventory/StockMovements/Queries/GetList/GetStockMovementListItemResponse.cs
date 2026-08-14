using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.Application.Inventory.StockMovements.Queries.GetList;

public sealed record GetStockMovementListItemResponse(
    int Id,
    int ProductId,
    int WarehouseId,
    int LocationId,
    decimal Quantity,
    StockMovementType MovementType);