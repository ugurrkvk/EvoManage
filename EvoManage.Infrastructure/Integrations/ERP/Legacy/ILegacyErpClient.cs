namespace EvoManage.Infrastructure.Integrations.ERP.Legacy;

public interface ILegacyErpClient
{
    Task SendStockTransactionAsync(LegacyErpStockRequest request, CancellationToken cancellationToken = default);
}