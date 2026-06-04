using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Provides access to the points promotion catalogue.</summary>
public interface IPointsPromotionRepository
{
    /// <summary>Returns all points promotions active on the specified date.</summary>
    IReadOnlyList<PointsPromotion> GetActiveOnDate(DateOnly date);
}
