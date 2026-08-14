namespace EvoManage.Application.Inventory.Stock.Queries.GetBalance;

public sealed record GetStockBalanceResponse(
    int ProductId,
    int WarehouseId,
    int LocationId,
    decimal Quantity);