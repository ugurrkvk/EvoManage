using EvoManage.LegacyErpSimulator.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EvoManage.LegacyErpSimulator.Controllers;

[ApiController]
[Route("api/legacy-stock-transactions")]
public sealed class LegacyStockTransactionsController(ILogger<LegacyStockTransactionsController> logger) : ControllerBase
{
    [HttpPost]
    public IActionResult Create(LegacyStockTransactionRequest request)
    {
        logger.LogInformation(
            "Legacy ERP received stock transaction. " +
            "ItemCode: {ItemCode}, " +
            "WarehouseNumber: {WarehouseNumber}, " +
            "TransactionAmount: {TransactionAmount}, " +
            "TransactionType: {TransactionType}",
            request.ItemCode,
            request.WarehouseNumber,
            request.TransactionAmount,
            request.TransactionType);

        return Ok();
    }
}