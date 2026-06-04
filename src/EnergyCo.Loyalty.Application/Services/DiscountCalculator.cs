using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Services;

/// <summary>Applies all active per-product discount promotions to a single basket line.</summary>
internal sealed class DiscountCalculator(IDiscountPromotionRepository discountPromotionRepository) : IDiscountCalculator
{
    public decimal Calculate(BasketItem item, DateOnly transactionDate)
    {
        var promotions = discountPromotionRepository.GetActiveOnDate(transactionDate);

        var discount = promotions
            .Where(p => p.AppliesToProduct(item.ProductCode))
            .Sum(p => p.CalculateLineDiscount(item.LineTotal.Amount));

        return Math.Round(discount, 2, MidpointRounding.AwayFromZero);
    }
}
