using EvoManage.Application.Integrations.ERP.Stock;
using EvoManage.Domain.Inventory.StockMovements;
using Microsoft.AspNetCore.Mvc;

namespace EvoManage.API.Controllers;

[ApiController]
[Route("api/integrations/erp/stock")]
public sealed class ErpIntegrationTestController(IErpStockIntegration erpStockIntegration) : ControllerBase
{
    [HttpPost("test")]
    public async Task<IActionResult> SendTestStockMovement(CancellationToken cancellationToken)
    {
        var movement = new ErpStockMovementModel(
            ProductId: 1,
            WarehouseId: 1,
            LocationId: 5,
            Quantity: 10m,
            MovementType: StockMovementType.Issue);
        await erpStockIntegration.SendStockMovementAsync(movement, cancellationToken);
        return Ok(new { message = "Stock movement sent to legacy ERP simulator." });
    }
}