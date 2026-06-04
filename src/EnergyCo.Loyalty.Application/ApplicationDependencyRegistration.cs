using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Baskets.Commands.AddToBasket;
using EnergyCo.Loyalty.Application.Baskets.Commands.CheckoutBasket;
using EnergyCo.Loyalty.Application.Baskets.Commands.ExpressCheckout;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Application.Dispatching;
using EnergyCo.Loyalty.Application.Products.Queries;
using EnergyCo.Loyalty.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EnergyCo.Loyalty.Application;

public static class ApplicationDependencyRegistration
{
    /// <summary>Registers Application layer services.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Dispatcher
        services.AddScoped<IDispatcher, Dispatcher>();

        // Services
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IDiscountCalculator, DiscountCalculator>();
        services.AddScoped<IPointsCalculator, PointsCalculator>();
        services.AddScoped<ICheckoutService, CheckoutService>();

        // Command handlers
        services.AddScoped<ICommandHandler<AddToBasketCommand, Result>, AddToBasketCommandHandler>();
        services.AddScoped<ICommandHandler<CheckoutBasketCommand, Result<CheckoutSummaryDto>>, CheckoutBasketCommandHandler>();
        services.AddScoped<ICommandHandler<ExpressCheckoutCommand, Result<CheckoutSummaryDto>>, ExpressCheckoutCommandHandler>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetBasketQuery, Result<BasketDto>>, GetBasketQueryHandler>();
        services.AddScoped<IQueryHandler<GetProductsQuery, Result<IReadOnlyList<ProductDto>>>, GetProductsQueryHandler>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(ApplicationDependencyRegistration).Assembly, includeInternalTypes: true);

        return services;
    }
}

