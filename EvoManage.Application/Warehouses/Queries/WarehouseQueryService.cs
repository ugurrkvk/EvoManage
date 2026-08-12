using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Warehouses.Queries.GetById;
using EvoManage.Application.Warehouses.Queries.GetList;

namespace EvoManage.Application.Warehouses.Queries;

public sealed class WarehouseQueryService(
    IWarehouseRepository warehouseRepository)
{
    public async Task<GetWarehouseByIdResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (warehouse is null)
            throw new NotFoundException(
                $"Warehouse with id '{id}' was not found.");

        return new GetWarehouseByIdResponse(
            warehouse.Id,
            warehouse.Code,
            warehouse.Name,
            warehouse.Address,
            warehouse.Description,
            warehouse.IsActive);
    }

    public async Task<GetWarehouseListResponse> GetListAsync(
        GetWarehouseListRequest request,
        CancellationToken cancellationToken = default)
    {
        var warehouses = await warehouseRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalCount = await warehouseRepository.CountAsync(
            cancellationToken);

        var items = warehouses
            .Select(warehouse => new GetWarehouseListItemResponse(
                warehouse.Id,
                warehouse.Code,
                warehouse.Name,
                warehouse.Address,
                warehouse.IsActive))
            .ToArray();

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)request.PageSize);

        return new GetWarehouseListResponse(
            items,
            request.PageNumber,
            request.PageSize,
            totalCount,
            totalPages);
    }
}