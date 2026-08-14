namespace EvoManage.Application.Inventory.Stock.Queries.GetList;

public sealed record GetStockListRequest(
    int? ProductId,
    int? WarehouseId,
    int? LocationId,
    bool IncludeZeroStock = false,
    int PageNumber = 1,
    int PageSize = 20);