using EvoManage.Application.Products.Commands;
using EvoManage.Application.Products.Queries;
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
        return services;
    }
}

