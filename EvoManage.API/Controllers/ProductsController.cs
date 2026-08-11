using EvoManage.API.ExceptionHandling;
using EvoManage.Application.Products.Activate;
using EvoManage.Application.Products.Create;
using EvoManage.Application.Products.Deactivate;
using EvoManage.Application.Products.Delete;
using EvoManage.Application.Products.GetById;
using EvoManage.Application.Products.GetList;
using EvoManage.Application.Products.Update;
using Microsoft.AspNetCore.Mvc;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    CreateProductService createProductService, GetProductByIdService getProductByIdService, GetProductListService getProductListService, UpdateProductService updateProductService, DeleteProductService deleteProductService, DeactivateProductService deactivateProductService, ActivateProductService activateProductService
    ) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var response = await createProductService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<GetProductByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProductByIdResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await getProductByIdService.GetAsync(
            id,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType<GetProductListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProductListResponse>> GetList(
        [FromQuery] GetProductListRequest request,
        CancellationToken cancellationToken)
    {
        var response = await getProductListService.GetAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        await updateProductService.UpdateAsync(
            id,
            request,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await deleteProductService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        await deactivateProductService.DeactivateAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Activate(
        int id,
        CancellationToken cancellationToken)
    {
        await activateProductService.ActivateAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}