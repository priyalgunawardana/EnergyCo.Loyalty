using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Infrastructure.SeedData;

namespace EnergyCo.Loyalty.Infrastructure.Repositories;

internal sealed class InMemoryDiscountPromotionRepository : IDiscountPromotionRepository
{
    private readonly IReadOnlyList<DiscountPromotion> _promotions = DiscountPromotionSeed.GetPromotions();

    public IReadOnlyList<DiscountPromotion> GetActiveOnDate(DateOnly date) =>
        _promotions.Where(p => p.IsActiveOn(date)).ToList();
}
