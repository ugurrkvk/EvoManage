using EvoManage.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EvoManage.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(product => product.Code)
                .IsUnique();

            builder.Property(product => product.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(product => product.TrackingType)
                .IsRequired();

            builder.Property(product => product.IsActive)
                .IsRequired();
        }
    }
}
