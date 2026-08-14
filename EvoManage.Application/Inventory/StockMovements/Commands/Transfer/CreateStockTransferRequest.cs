namespace EvoManage.Application.Inventory.StockMovements.Commands.Transfer;

public sealed record CreateStockTransferRequest(
    int ProductId,
    int SourceWarehouseId,
    int SourceLocationId,
    int TargetWarehouseId,
    int TargetLocationId,
    decimal Quantity);