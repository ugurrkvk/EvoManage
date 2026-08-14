using EvoManage.Domain.Inventory.StockMovements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoManage.Infrastructure.Persistence.Configurations;

public sealed class StockMovementConfiguration
    : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(movement => movement.Id);

        builder.Property(movement => movement.ProductId)
            .IsRequired();

        builder.Property(movement => movement.WarehouseId)
            .IsRequired();

        builder.Property(movement => movement.LocationId)
            .IsRequired();

        builder.Property(movement => movement.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(movement => movement.MovementType)
            .IsRequired();

        builder.Ignore(movement => movement.SignedQuantity);

        builder.HasIndex(movement => new
        {
            movement.ProductId,
            movement.WarehouseId,
            movement.LocationId
        });
    }
}