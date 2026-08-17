namespace EvoManage.LegacyErpSimulator.Contracts;

public sealed record LegacyStockTransactionRequest(
    string ItemCode,
    short WarehouseNumber,
    decimal TransactionAmount,
    string TransactionType);