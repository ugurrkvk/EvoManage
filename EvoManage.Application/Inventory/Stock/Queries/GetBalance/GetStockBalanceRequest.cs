namespace EvoManage.Application.Inventory.Stock.Queries.GetBalance;

public sealed record GetStockBalanceRequest(
    int ProductId,
    int WarehouseId,
    int LocationId);