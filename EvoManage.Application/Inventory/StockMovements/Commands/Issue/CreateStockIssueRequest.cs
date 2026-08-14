namespace EvoManage.Application.Inventory.StockMovements.Commands.Issue;

public sealed record CreateStockIssueRequest(
    int ProductId,
    int WarehouseId,
    int LocationId,
    decimal Quantity);