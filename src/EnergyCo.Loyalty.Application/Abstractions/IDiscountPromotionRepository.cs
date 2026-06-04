using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Provides access to the discount promotion catalogue.</summary>
public interface IDiscountPromotionRepository
{
    /// <summary>Returns all discount promotions that are active on the specified transaction date.</summary>
    IReadOnlyList<DiscountPromotion> GetActiveOnDate(DateOnly date);
}
