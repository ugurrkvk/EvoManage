using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Common.Exceptions;
using EvoManage.Application.Warehouses.Commands.Create;
using EvoManage.Application.Warehouses.Commands.Update;
using EvoManage.Domain.Warehouses;

namespace EvoManage.Application.Warehouses.Commands;

public sealed class WarehouseCommandService(
    IWarehouseRepository warehouseRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateWarehouseResponse> CreateAsync(
        CreateWarehouseRequest request,
        CancellationToken cancellationToken = default)
    {
        var exists = await warehouseRepository.ExistsByCodeAsync(
            request.Code,
            cancellationToken);

        if (exists)
            throw new ConflictException(
                $"Warehouse code '{request.Code}' already exists.");

        var warehouse = Warehouse.Create(
            request.Code,
            request.Name,
            request.Address,
            request.Description);

        await warehouseRepository.AddAsync(
            warehouse,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateWarehouseResponse(warehouse.Id);
    }

    public async Task UpdateAsync(
        int id,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await GetWarehouseOrThrowAsync(
            id,
            cancellationToken);

        var codeExists = await warehouseRepository.ExistsByCodeExceptIdAsync(
            request.Code,
            id,
            cancellationToken);

        if (codeExists)
            throw new ConflictException(
                $"Warehouse code '{request.Code}' already exists.");

        warehouse.Update(
            request.Code,
            request.Name,
            request.Address,
            request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await GetWarehouseOrThrowAsync(
            id,
            cancellationToken);

        warehouseRepository.Remove(warehouse);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await GetWarehouseOrThrowAsync(
            id,
            cancellationToken);

        warehouse.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await GetWarehouseOrThrowAsync(
            id,
            cancellationToken);

        warehouse.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Warehouse> GetWarehouseOrThrowAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await warehouseRepository.GetByIdAsync(
                   id,
                   cancellationToken)
               ?? throw new NotFoundException(
                   $"Warehouse with id '{id}' was not found.");
    }
}