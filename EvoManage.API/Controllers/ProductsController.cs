using EvoManage.API.ExceptionHandling;
using EvoManage.Application.Products.Commands;
using EvoManage.Application.Products.Queries;
using Microsoft.AspNetCore.Mvc;
using EvoManage.API.OpenApi;
using EvoManage.Application.Products.Commands.Create;
using EvoManage.Application.Products.Commands.Update;
using EvoManage.Application.Products.Queries.GetById;
using EvoManage.Application.Products.Queries.GetList;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(ProductCommandService productCommandService, ProductQueryService productQueryService) : ControllerBase
{
    [HttpPost]
    [ApiErrors(400, 409, 422)]
    public async Task<ActionResult<CreateProductResponse>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await productCommandService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:int}")]
    [ApiErrors(404)]
    public async Task<ActionResult<GetProductByIdResponse>> GetById(int id, CancellationToken cancellationToken) => await productQueryService.GetByIdAsync(id, cancellationToken);

    [HttpGet]
    [ApiErrors(400)]
    public async Task<ActionResult<GetProductListResponse>> GetList([FromQuery] GetProductListRequest request, CancellationToken cancellationToken)=> await productQueryService.GetListAsync(request, cancellationToken);

    [HttpPut("{id:int}")]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<IActionResult> Update(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        await productCommandService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ApiErrors(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await productCommandService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    [ApiErrors(404)]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        await productCommandService.ActivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/deactivate")]
    [ApiErrors(404)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await productCommandService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }
}