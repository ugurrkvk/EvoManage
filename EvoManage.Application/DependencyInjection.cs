using EvoManage.Application.Products.Activate;
using EvoManage.Application.Products.Create;
using EvoManage.Application.Products.Deactivate;
using EvoManage.Application.Products.Delete;
using EvoManage.Application.Products.GetById;
using EvoManage.Application.Products.GetList;
using EvoManage.Application.Products.Update;
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
        services.AddScoped<GetProductByIdService>();
        services.AddScoped<GetProductListService>();
        services.AddScoped<UpdateProductService>();
        services.AddScoped<DeleteProductService>();
        services.AddScoped<DeactivateProductService>();
        services.AddScoped<ActivateProductService>();
        return services;
    }
}

