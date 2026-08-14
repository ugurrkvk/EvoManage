using EvoManage.API.OpenApi;
using EvoManage.Application.Inventory.Stock.Queries;
using EvoManage.Application.Inventory.Stock.Queries.GetBalance;
using Microsoft.AspNetCore.Mvc;
using EvoManage.Application.Inventory.Stock.Queries.GetList;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/stocks")]
public sealed class StocksController(
    StockQueryService stockQueryService) : ControllerBase
{
    [HttpGet("balance")]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<ActionResult<GetStockBalanceResponse>> GetBalance(
        [FromQuery] GetStockBalanceRequest request,
        CancellationToken cancellationToken)
        => await stockQueryService.GetBalanceAsync(
            request,
            cancellationToken);

    [HttpGet]
    [ApiErrors(400, 422)]
    public async Task<ActionResult<GetStockListResponse>> GetList(
        [FromQuery] GetStockListRequest request,
        CancellationToken cancellationToken)
    {
        var response = await stockQueryService.GetListAsync(
            request,
            cancellationToken);

        return Ok(response);
    }
}
