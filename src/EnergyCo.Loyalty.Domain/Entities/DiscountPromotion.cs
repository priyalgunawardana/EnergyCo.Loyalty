namespace EnergyCo.Loyalty.Domain.Entities;

/// <summary>
/// A promotion that applies a percentage discount to qualifying products only.
/// Eligibility is per-product (by Code), not basket-wide.
/// </summary>
public class DiscountPromotion
{
    private readonly HashSet<string> _eligibleProductCodes;

    /// <summary>Business identifier, e.g. "DP001".</summary>
    public string Id { get; } // Better use guid with a code property, but this is simpler for the given data and tests.

    public string Name { get; }

    public DateOnly StartDate { get; }

    public DateOnly EndDate { get; }

    /// <summary>Discount percentage, e.g. 20 means 20%.</summary>
    public decimal DiscountPercent { get; }

    /// <summary>The product codes (e.g. "PRD02") that are eligible for this discount.</summary>
    public IReadOnlySet<string> EligibleProductCodes => _eligibleProductCodes;

    public DiscountPromotion(
        string id,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        decimal discountPercent,
        IEnumerable<string> eligibleProductCodes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (discountPercent <= 0 || discountPercent > 100)
            throw new ArgumentOutOfRangeException(nameof(discountPercent));
        ArgumentNullException.ThrowIfNull(eligibleProductCodes);

        Id = id;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        DiscountPercent = discountPercent;
        _eligibleProductCodes = new HashSet<string>(eligibleProductCodes, StringComparer.OrdinalIgnoreCase);

        if (_eligibleProductCodes.Count == 0)
            throw new ArgumentException("A discount promotion must have at least one eligible product.", nameof(eligibleProductCodes));
    }

    /// <summary>Returns true if this promotion is active on the given transaction date.</summary>
    public bool IsActiveOn(DateOnly date) => date >= StartDate && date <= EndDate;

    /// <summary>Returns true if the given product code is eligible for this discount.</summary>
    public bool AppliesToProduct(string productCode) =>
        _eligibleProductCodes.Contains(productCode);

    /// <summary>Calculates the discount amount for a single item line total.</summary>
    public decimal CalculateLineDiscount(decimal lineTotal) =>
        Math.Round(lineTotal * DiscountPercent / 100m, 2, MidpointRounding.AwayFromZero);
}
