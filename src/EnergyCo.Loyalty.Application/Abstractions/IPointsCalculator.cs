using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Calculates the reward points earned for a single basket line.</summary>
public interface IPointsCalculator
{
    /// <summary>
    /// Returns the points earned for <paramref name="item"/> given the promotions active on <paramref name="transactionDate"/>.
    /// </summary>
    int Calculate(BasketItem item, DateOnly transactionDate);
}
