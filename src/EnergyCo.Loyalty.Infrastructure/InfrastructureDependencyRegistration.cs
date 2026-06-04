using EnergyCo.Loyalty.Application;
using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EnergyCo.Loyalty.Infrastructure;

public static class InfrastructureDependencyRegistration
{
    /// <summary>Registers Infrastructure layer services.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddApplication();

        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddSingleton<IBasketRepository, InMemoryBasketRepository>();
        services.AddSingleton<IDiscountPromotionRepository, InMemoryDiscountPromotionRepository>();
        services.AddSingleton<IPointsPromotionRepository, InMemoryPointsPromotionRepository>();

        return services;
    }
}
