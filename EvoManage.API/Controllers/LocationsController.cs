using EvoManage.API.OpenApi;
using EvoManage.Application.Locations.Commands;
using EvoManage.Application.Locations.Commands.Create;
using EvoManage.Application.Locations.Commands.Update;
using EvoManage.Application.Locations.Queries;
using EvoManage.Application.Locations.Queries.GetById;
using EvoManage.Application.Locations.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController(
    LocationCommandService locationCommandService,
    LocationQueryService locationQueryService) : ControllerBase
{
    [HttpPost]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<ActionResult<CreateLocationResponse>> Create(
        CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await locationCommandService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    [HttpGet("{id:int}")]
    [ApiErrors(404)]
    public async Task<ActionResult<GetLocationByIdResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
        => await locationQueryService.GetByIdAsync(
            id,
            cancellationToken);

    [HttpGet]
    [ApiErrors(400)]
    public async Task<ActionResult<GetLocationListResponse>> GetList(
        [FromQuery] GetLocationListRequest request,
        CancellationToken cancellationToken)
        => await locationQueryService.GetListAsync(
            request,
            cancellationToken);

    [HttpPut("{id:int}")]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<IActionResult> Update(
        int id,
        UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        await locationCommandService.UpdateAsync(
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
        await locationCommandService.DeleteAsync(
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
        await locationCommandService.ActivateAsync(
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
        await locationCommandService.DeactivateAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}