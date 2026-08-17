using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.Application.Inventory.StockMovements.Events;

public sealed record StockMovementCreatedEvent(
    int MovementId,
    int ProductId,
    int WarehouseId,
    int LocationId,
    decimal Quantity,
    StockMovementType MovementType);