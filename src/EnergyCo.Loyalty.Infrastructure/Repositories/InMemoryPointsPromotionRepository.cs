using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Infrastructure.SeedData;

namespace EnergyCo.Loyalty.Infrastructure.Repositories;

internal sealed class InMemoryPointsPromotionRepository : IPointsPromotionRepository
{
    private readonly IReadOnlyList<PointsPromotion> _promotions = PointsPromotionSeed.GetPromotions();

    public IReadOnlyList<PointsPromotion> GetActiveOnDate(DateOnly date) =>
        _promotions.Where(p => p.IsActiveOn(date)).ToList();
}
