using EvoManage.Application.Products.Create;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EvoManage.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateProductService>();
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);
        return services;
    }
}

