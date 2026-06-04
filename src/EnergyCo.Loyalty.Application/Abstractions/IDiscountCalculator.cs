using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Calculates the total discount amount for a single basket line.</summary>
public interface IDiscountCalculator
{
    /// <summary>
    /// Returns the discount amount for <paramref name="item"/> given the promotions active on <paramref name="transactionDate"/>.
    /// </summary>
    decimal Calculate(BasketItem item, DateOnly transactionDate);
}
