namespace EvoManage.Application.Inventory.StockMovements.Commands.Transfer;

public sealed record CreateStockTransferResponse(
    int TransferOutMovementId,
    int TransferInMovementId);