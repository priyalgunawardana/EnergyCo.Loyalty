using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Computes the final checkout summary: discounts, totals, and reward points.</summary>
public interface ICheckoutService
{
    Task<Result<CheckoutSummaryDto>> ProcessAsync(Basket basket, string loyaltyCard, DateOnly transactionDate, CancellationToken cancellationToken = default);
}
