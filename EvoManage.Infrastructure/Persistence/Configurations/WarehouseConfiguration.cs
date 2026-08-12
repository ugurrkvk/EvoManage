using EvoManage.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoManage.Infrastructure.Persistence.Configurations;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(warehouse => warehouse.Id);

        builder.Property(warehouse => warehouse.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(warehouse => warehouse.Code)
            .IsUnique();

        builder.Property(warehouse => warehouse.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(warehouse => warehouse.Address)
            .HasMaxLength(500);

        builder.Property(warehouse => warehouse.Description)
            .HasMaxLength(1000);

        builder.Property(warehouse => warehouse.IsActive)
            .IsRequired();
    }
}