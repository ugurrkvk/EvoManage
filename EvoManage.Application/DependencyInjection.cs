using EvoManage.Application.Products.Commands;
using EvoManage.Application.Products.Queries;
using EvoManage.Application.Warehouses.Commands;
using EvoManage.Application.Warehouses.Queries;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EvoManage.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ProductCommandService>();
        services.AddScoped<ProductQueryService>();
        services.AddScoped<WarehouseCommandService>();
        services.AddScoped<WarehouseQueryService>();
        return services;
    }
}

