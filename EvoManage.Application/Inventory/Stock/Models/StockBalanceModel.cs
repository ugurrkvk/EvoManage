namespace EvoManage.Application.Inventory.Stocks.Models;

public sealed record StockBalanceModel(
    int ProductId,
    string ProductCode,
    string ProductName,
    int WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    int LocationId,
    string LocationCode,
    decimal Quantity);