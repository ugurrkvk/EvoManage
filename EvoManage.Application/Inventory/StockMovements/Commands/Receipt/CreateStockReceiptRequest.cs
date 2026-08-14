namespace EvoManage.Application.Inventory.StockMovements.Commands.Receipt;

public sealed record CreateStockReceiptRequest(
    int ProductId,
    int WarehouseId,
    int LocationId,
    decimal Quantity);