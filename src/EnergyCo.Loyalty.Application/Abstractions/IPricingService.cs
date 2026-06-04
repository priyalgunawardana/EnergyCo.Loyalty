using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Validates a submitted price against the current product catalogue.</summary>
public interface IPricingService
{
    Task<Result<Product>> ValidatePriceAsync(string productCode, decimal submittedPrice, CancellationToken cancellationToken = default);
}
