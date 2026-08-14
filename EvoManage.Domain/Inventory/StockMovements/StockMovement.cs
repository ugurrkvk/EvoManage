using EvoManage.Domain.Common;
using EvoManage.Domain.Common.Exceptions;

namespace EvoManage.Domain.Inventory.StockMovements;

public sealed class StockMovement : BaseEntity
{
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int LocationId { get; private set; }
    public decimal Quantity { get; private set; }
    public StockMovementType MovementType { get; private set; }

    private StockMovement()
    {
    }

    public static StockMovement Create(
        int productId,
        int warehouseId,
        int locationId,
        decimal quantity,
        StockMovementType movementType)
    {
        Validate(
            productId,
            warehouseId,
            locationId,
            quantity,
            movementType);

        return new StockMovement
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            LocationId = locationId,
            Quantity = quantity,
            MovementType = movementType
        };
    }

    private static void Validate(
        int productId,
        int warehouseId,
        int locationId,
        decimal quantity,
        StockMovementType movementType)
    {
        if (productId <= 0)
            throw new DomainException(
                "Product id must be greater than zero.");

        if (warehouseId <= 0)
            throw new DomainException(
                "Warehouse id must be greater than zero.");

        if (locationId <= 0)
            throw new DomainException(
                "Location id must be greater than zero.");

        if (quantity <= 0)
            throw new DomainException(
                "Stock movement quantity must be greater than zero.");

        if (!Enum.IsDefined(movementType))
            throw new DomainException(
                "Stock movement type is invalid.");
    }

    public decimal SignedQuantity => MovementType switch
    {
        StockMovementType.Receipt => Quantity,
        StockMovementType.TransferIn => Quantity,
        StockMovementType.Issue => -Quantity,
        StockMovementType.TransferOut => -Quantity,
        _ => throw new DomainException("Stock movement type is invalid.")
    };
}