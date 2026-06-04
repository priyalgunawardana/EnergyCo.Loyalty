using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Basket lifecycle operations: add items, retrieve, and remove.</summary>
public interface IBasketService
{
    Task<Result<Basket>> AddItemAsync(string customerId, string productCode, decimal unitPrice, int quantity, CancellationToken cancellationToken = default);
    Task<Basket?> FindAsync(string customerId, CancellationToken cancellationToken = default);
    Task RemoveAsync(string customerId, CancellationToken cancellationToken = default);
}
