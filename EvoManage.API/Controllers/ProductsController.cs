using EvoManage.Application.Products.Create;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using EvoManage.API.ExceptionHandling;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    CreateProductService createProductService
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

        return StatusCode(StatusCodes.Status201Created, response);
    }
}