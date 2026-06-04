using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Infrastructure.SeedData;

internal static class PointsPromotionSeed
{
    // The points promotions include a mix of category-specific and general promotions, with unique codes to match with given data.
    public static IReadOnlyList<PointsPromotion> GetPromotions() =>
    [
        new PointsPromotion("PP001", "New Year Promo",
            new DateOnly(2020, 1,  1), new DateOnly(2020, 1, 30),
            category: "Any",  pointsPerUnitSpent: 2),

        new PointsPromotion("PP002", "Fuel Promo",
            new DateOnly(2020, 2,  5), new DateOnly(2020, 2, 15),
            category: "Fuel", pointsPerUnitSpent: 3),

        new PointsPromotion("PP003", "Shop Promo",
            new DateOnly(2020, 3,  1), new DateOnly(2020, 3, 20),
            category: "Shop", pointsPerUnitSpent: 4),
    ];
}
