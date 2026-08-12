using EvoManage.API.OpenApi;
using EvoManage.Application.Warehouses.Commands;
using EvoManage.Application.Warehouses.Commands.Create;
using EvoManage.Application.Warehouses.Commands.Update;
using EvoManage.Application.Warehouses.Queries;
using EvoManage.Application.Warehouses.Queries.GetById;
using EvoManage.Application.Warehouses.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/warehouses")]
public sealed class WarehousesController(
    WarehouseCommandService warehouseCommandService,
    WarehouseQueryService warehouseQueryService) : ControllerBase
{
    [HttpPost]
    [ApiErrors(400, 409, 422)]
    public async Task<ActionResult<CreateWarehouseResponse>> Create(
        CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await warehouseCommandService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    [HttpGet("{id:int}")]
    [ApiErrors(404)]
    public async Task<ActionResult<GetWarehouseByIdResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
        => await warehouseQueryService.GetByIdAsync(id, cancellationToken);

    [HttpGet]
    [ApiErrors(400)]
    public async Task<ActionResult<GetWarehouseListResponse>> GetList(
        [FromQuery] GetWarehouseListRequest request,
        CancellationToken cancellationToken)
        => await warehouseQueryService.GetListAsync(
            request,
            cancellationToken);

    [HttpPut("{id:int}")]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<IActionResult> Update(
        int id,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        await warehouseCommandService.UpdateAsync(
            id,
            request,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ApiErrors(404)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await warehouseCommandService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    [ApiErrors(404)]
    public async Task<IActionResult> Activate(
        int id,
        CancellationToken cancellationToken)
    {
        await warehouseCommandService.ActivateAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/deactivate")]
    [ApiErrors(404)]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        await warehouseCommandService.DeactivateAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}