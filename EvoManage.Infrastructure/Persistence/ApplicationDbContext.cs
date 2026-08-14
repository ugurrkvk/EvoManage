using EvoManage.Domain.Inventory.StockMovements;
using EvoManage.Domain.Locations;
using EvoManage.Domain.Products;
using EvoManage.Domain.Warehouses;
using EvoManage.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace EvoManage.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

