namespace EvoManage.Application.Inventory.Stock.Queries.GetList;

public sealed record GetStockListItemResponse(
    int ProductId,
    string ProductCode,
    string ProductName,
    int WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    int LocationId,
    string LocationCode,
    decimal Quantity);