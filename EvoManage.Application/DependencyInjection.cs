using EvoManage.Application.Inventory.Common.StockAllocation;
using EvoManage.Application.Inventory.Common.StockAllocation.Strategies;
using EvoManage.Application.Inventory.Stock.Queries;
using EvoManage.Application.Inventory.StockMovements.Commands;
using EvoManage.Application.Inventory.StockMovements.Events;
using EvoManage.Application.Inventory.StockMovements.Events.Handlers;
using EvoManage.Application.Inventory.StockMovements.Queries;
using EvoManage.Application.Inventory.StockMovements.Validation.Common;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Handlers;
using EvoManage.Application.Inventory.StockMovements.Validation.Common.Validators;
using EvoManage.Application.Inventory.StockMovements.Validation.Transfer;
using EvoManage.Application.Inventory.StockMovements.Validation.Transfer.Handlers;
using EvoManage.Application.Locations.Commands;
using EvoManage.Application.Locations.Queries;
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
        services.AddScoped<LocationCommandService>();
        services.AddScoped<LocationQueryService>();
        services.AddScoped<StockMovementCommandService>();
        services.AddScoped<StockMovementQueryService>();
        services.AddScoped<StockQueryService>();
        services.AddScoped<IStockAllocationStrategy, ManualLocationAllocationStrategy>();
        services.AddScoped<IStockAllocationStrategy, HighestStockAllocationStrategy>();
        services.AddScoped<IStockAllocationStrategy, LowestStockAllocationStrategy>();
        services.AddScoped<StockAllocationStrategyResolver>();
        services.AddScoped<StockMovementCreatedEventDispatcher>();
        services.AddScoped<IStockMovementCreatedEventHandler, ErpStockMovementCreatedEventHandler>();
        services.AddScoped<IStockMovementValidationHandler, ProductValidationHandler>();
        services.AddScoped<IStockMovementValidationHandler, WarehouseValidationHandler>();
        services.AddScoped<IStockMovementValidationHandler, LocationValidationHandler>();
        services.AddScoped<IActiveProductValidator, ActiveProductValidator>();
        services.AddScoped<IActiveWarehouseValidator, ActiveWarehouseValidator>();
        services.AddScoped<IWarehouseLocationValidator, WarehouseLocationValidator>();
        services.AddScoped<StockMovementValidationPipeline>();
        services.AddScoped<IStockTransferValidationHandler, TransferProductValidationHandler>();
        services.AddScoped<IStockTransferValidationHandler, SourceWarehouseValidationHandler>();
        services.AddScoped<IStockTransferValidationHandler, SourceLocationValidationHandler>();
        services.AddScoped<IStockTransferValidationHandler, TargetWarehouseValidationHandler>();
        services.AddScoped<IStockTransferValidationHandler, TargetLocationValidationHandler>();
        services.AddScoped<IStockTransferValidationHandler, DifferentSourceAndTargetValidationHandler>();
        services.AddScoped<IStockTransferValidationHandler, SufficientStockValidationHandler>();
        services.AddScoped<StockTransferValidationPipeline>();

        return services;
    }
}

