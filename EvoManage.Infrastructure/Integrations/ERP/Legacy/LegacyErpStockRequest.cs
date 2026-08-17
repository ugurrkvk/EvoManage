namespace EvoManage.Infrastructure.Integrations.ERP.Legacy;

public sealed record LegacyErpStockRequest(
    string ItemCode,
    short WarehouseNumber,
    decimal TransactionAmount,
    string TransactionType);