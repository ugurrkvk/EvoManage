using EvoManage.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoManage.Infrastructure.Persistence.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(location => location.Id);

        builder.Property(location => location.WarehouseId)
            .IsRequired();

        builder.Property(location => location.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(location => location.GroupCode)
            .HasMaxLength(50);

        builder.Property(location => location.IsActive)
            .IsRequired();

        builder.HasIndex(location => new
            {
                location.WarehouseId,
                location.Code
            })
            .IsUnique();
    }
}