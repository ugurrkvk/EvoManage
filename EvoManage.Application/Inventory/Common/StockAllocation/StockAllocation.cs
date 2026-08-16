namespace EvoManage.Application.Inventory.Common.StockAllocation;

public sealed record StockAllocation(int WarehouseId, int LocationId, decimal Quantity);