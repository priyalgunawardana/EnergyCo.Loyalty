namespace EnergyCo.Loyalty.Domain.Entities;

/// <summary>A promotion that awards loyalty points per currency unit spent, optionally scoped to a product category.</summary>
public class PointsPromotion
{
    /// <summary>Business identifier, e.g. PP001.</summary>
    public string Id { get; } // Better use guid with a code property, but this is simpler for the given data and tests.

    public string Name { get; }

    public DateOnly StartDate { get; }

    public DateOnly EndDate { get; }

    /// <summary>The product category this promotion applies to, or "Any" for all categories.</summary>
    public string Category { get; }

    /// <summary>Points awarded for each whole currency unit ($) spent on qualifying items.</summary>
    public int PointsPerUnitSpent { get; }

    public PointsPromotion(string id, string name, DateOnly startDate, DateOnly endDate, string category, int pointsPerUnitSpent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        if (pointsPerUnitSpent <= 0) throw new ArgumentOutOfRangeException(nameof(pointsPerUnitSpent));

        Id = id;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Category = category;
        PointsPerUnitSpent = pointsPerUnitSpent;
    }

    /// <summary>Returns true if this promotion is active on the given date.</summary>
    public bool IsActiveOn(DateOnly date) => date >= StartDate && date <= EndDate;

    /// <summary>Returns true if this promotion applies to the given product category.</summary>
    public bool AppliesToCategory(string category) =>
        Category.Equals("Any", StringComparison.OrdinalIgnoreCase) ||
        Category.Equals(category, StringComparison.OrdinalIgnoreCase);
}
