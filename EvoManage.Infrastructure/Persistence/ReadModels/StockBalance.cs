namespace EvoManage.Infrastructure.Persistence.ReadModels;

public sealed class StockBalance
{
    public int ProductId { get; init; }

    public string ProductCode { get; init; } = null!;

    public string ProductName { get; init; } = null!;

    public int WarehouseId { get; init; }

    public string WarehouseCode { get; init; } = null!;

    public string WarehouseName { get; init; } = null!;

    public int LocationId { get; init; }

    public string LocationCode { get; init; } = null!;

    public decimal Quantity { get; init; }
}