using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Infrastructure.SeedData;

internal static class DiscountPromotionSeed
{
    public static IReadOnlyList<DiscountPromotion> GetPromotions() =>
    [
        // Assumption: DP001 product list shows PRD02 twice, which is a data entry error.
        // Assumed to cover PRD01 (Vortex 95) and PRD02 (Vortex 98) — both Fuel products matching the "Fuel Discount Promo" name.
        // The "Happy Promo" applies to all Shop products, which are PRD04-PRD08.
        new DiscountPromotion(
            "DP001",
            "Fuel Discount Promo",
            new DateOnly(2020, 1,  1),
            new DateOnly(2020, 2, 15),
            discountPercent: 20m,
            eligibleProductCodes: ["PRD01", "PRD02"]),

        new DiscountPromotion(
            "DP002",
            "Happy Promo",
            new DateOnly(2020, 3,  2),
            new DateOnly(2020, 3, 20),
            discountPercent: 15m,
            eligibleProductCodes: ["PRD04", "PRD05", "PRD06", "PRD07", "PRD08"])
    ];
}
