using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Repository for basket data.</summary>
public interface IBasketRepository
{
    Task<Basket?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
    Task SaveAsync(Basket basket, CancellationToken cancellationToken = default);
    Task DeleteAsync(string customerId, CancellationToken cancellationToken = default);
}
