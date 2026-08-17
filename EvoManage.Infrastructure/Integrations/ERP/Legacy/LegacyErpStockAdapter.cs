using EvoManage.Application.Integrations.ERP.Stock;
using EvoManage.Domain.Inventory.StockMovements;

namespace EvoManage.Infrastructure.Integrations.ERP.Legacy;

public sealed class LegacyErpStockAdapter(ILegacyErpClient legacyErpClient) : IErpStockIntegration
{
    public async Task SendStockMovementAsync(ErpStockMovementModel movement, CancellationToken cancellationToken = default)
    {
        var request = new LegacyErpStockRequest(
            ItemCode: movement.ProductId.ToString(),
            WarehouseNumber: checked((short)movement.WarehouseId),
            TransactionAmount: movement.Quantity,
            TransactionType: GetTransactionType(movement.MovementType));
        await legacyErpClient.SendStockTransactionAsync(request, cancellationToken);
    }

    private static string GetTransactionType(StockMovementType movementType)
    {
        return movementType switch
        {
            StockMovementType.Receipt => "IN",
            StockMovementType.Issue => "OUT",
            StockMovementType.TransferIn => "TRANSFER_IN",
            StockMovementType.TransferOut => "TRANSFER_OUT",
            _ => throw new ArgumentOutOfRangeException(nameof(movementType), movementType, "Unsupported stock movement type.")
        };
    }
}