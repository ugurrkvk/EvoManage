using EvoManage.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoManage.Infrastructure.Persistence.Configurations;

public sealed class StockBalanceConfiguration
    : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.HasNoKey();

        builder.ToView("StockBalanceView");

        builder.Property(stock => stock.ProductCode)
            .HasMaxLength(50);

        builder.Property(stock => stock.ProductName)
            .HasMaxLength(200);

        builder.Property(stock => stock.WarehouseCode)
            .HasMaxLength(50);

        builder.Property(stock => stock.WarehouseName)
            .HasMaxLength(200);

        builder.Property(stock => stock.LocationCode)
            .HasMaxLength(50);

        builder.Property(stock => stock.Quantity)
            .HasPrecision(18, 4);
    }
}