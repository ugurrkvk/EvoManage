namespace EvoManage.Application.Inventory.StockMovements.Commands.Issue;

public sealed record CreateStockIssueMovementResponse(int Id, int LocationId, decimal Quantity);

public sealed record CreateStockIssueResponse(IReadOnlyCollection<CreateStockIssueMovementResponse> Movements);