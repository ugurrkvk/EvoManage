using EvoManage.API.OpenApi;
using EvoManage.Application.Inventory.StockMovements.Commands;
using EvoManage.Application.Inventory.StockMovements.Commands.Issue;
using EvoManage.Application.Inventory.StockMovements.Commands.Receipt;
using EvoManage.Application.Inventory.StockMovements.Commands.Transfer;
using EvoManage.Application.Inventory.StockMovements.Queries;
using EvoManage.Application.Inventory.StockMovements.Queries.GetList;
using Microsoft.AspNetCore.Mvc;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/stock-movements")]
public sealed class StockMovementsController(
    StockMovementCommandService stockMovementCommandService,
    StockMovementQueryService stockMovementQueryService) : ControllerBase
{
    [HttpPost("receipts")]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<ActionResult<CreateStockReceiptResponse>> CreateReceipt(
        CreateStockReceiptRequest request,
        CancellationToken cancellationToken)
    {
        var response = await stockMovementCommandService.CreateReceiptAsync(
            request,
            cancellationToken);

        return Created(
            $"/api/stock-movements/{response.Id}",
            response);
    }

    [HttpPost("issues")]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<ActionResult<CreateStockIssueResponse>> CreateIssue(
        CreateStockIssueRequest request,
        CancellationToken cancellationToken)
    {
        var response = await stockMovementCommandService.CreateIssueAsync(request, cancellationToken);
        return Created("/api/stock-movements", response);
    }

    [HttpPost("transfers")]
    [ApiErrors(400, 404, 409, 422)]
    public async Task<ActionResult<CreateStockTransferResponse>> CreateTransfer(
        CreateStockTransferRequest request,
        CancellationToken cancellationToken)
    {
        var response = await stockMovementCommandService.CreateTransferAsync(
            request,
            cancellationToken);

        return Created(
            $"/api/stock-movements/{response.TransferOutMovementId}",
            response);
    }

    [HttpGet]
    [ApiErrors(400, 422)]
    public async Task<ActionResult<GetStockMovementListResponse>> GetList(
        [FromQuery] GetStockMovementListRequest request,
        CancellationToken cancellationToken)
    {
        var response = await stockMovementQueryService.GetListAsync(
            request,
            cancellationToken);

        return Ok(response);
    }
}