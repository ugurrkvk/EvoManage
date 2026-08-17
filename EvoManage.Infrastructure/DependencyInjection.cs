using EvoManage.Application.Abstractions.Persistence;
using EvoManage.Application.Abstractions.Persistence.Repositories;
using EvoManage.Application.Integrations.ERP.Stock;
using EvoManage.Infrastructure.Integrations.ERP.Legacy;
using EvoManage.Infrastructure.Persistence;
using EvoManage.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvoManage.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'SqlServer' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IStockReadRepository, StockReadRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var legacyErpBaseUrl = configuration["LegacyErp:BaseUrl"] ?? throw new InvalidOperationException("Legacy ERP base URL is not configured.");
        services.AddHttpClient<ILegacyErpClient, FakeLegacyErpClient>(client =>
        {
            client.BaseAddress = new Uri(legacyErpBaseUrl);
        });
        services.AddScoped<IErpStockIntegration, LegacyErpStockAdapter>();

        return services;
    }
}

