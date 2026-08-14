using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.Application.Inventory.StockMovements.Queries.GetList;

public sealed record GetStockMovementListRequest(
    int? ProductId,
    int? WarehouseId,
    int? LocationId,
    StockMovementType? MovementType,
    int PageNumber = 1,
    int PageSize = 20);