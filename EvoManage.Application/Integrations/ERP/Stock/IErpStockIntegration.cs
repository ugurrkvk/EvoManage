namespace EvoManage.Application.Integrations.ERP.Stock;

public interface IErpStockIntegration
{
    Task SendStockMovementAsync(ErpStockMovementModel movement, CancellationToken cancellationToken = default);
}