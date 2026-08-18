namespace EvoManage.Application.Inventory.StockMovements.Validation.Transfer;

public enum StockTransferValidationStep
{
    Product = 100,
    SourceWarehouse = 200,
    SourceLocation = 300,
    TargetWarehouse = 400,
    TargetLocation = 500,
    DifferentSourceTarget = 600,
    SufficientStock = 700
}