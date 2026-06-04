using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Services;

/// <summary>Awards points per whole currency unit spent, using the first matching category promotion since only one points promo can run at any given time.</summary>
internal sealed class PointsCalculator(IPointsPromotionRepository pointsPromotionRepository) : IPointsCalculator
{
    public int Calculate(BasketItem item, DateOnly transactionDate)
    {
        var promotion = pointsPromotionRepository
            .GetActiveOnDate(transactionDate)
            .FirstOrDefault(p => p.AppliesToCategory(item.Category));

        if (promotion is null)
            return 0;

        return (int)Math.Floor(item.LineTotal.Amount) * promotion.PointsPerUnitSpent;
    }
}
