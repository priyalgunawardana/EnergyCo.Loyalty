using System.Collections.Concurrent;
using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Infrastructure.Repositories;

internal sealed class InMemoryBasketRepository : IBasketRepository
{
    private readonly ConcurrentDictionary<string, Basket> _baskets = new();

    public Task<Basket?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        => Task.FromResult(_baskets.TryGetValue(customerId, out var basket) ? basket : null);

    public Task SaveAsync(Basket basket, CancellationToken cancellationToken = default)
    {
        _baskets[basket.CustomerId] = basket;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string customerId, CancellationToken cancellationToken = default)
    {
        _baskets.TryRemove(customerId, out _);
        return Task.CompletedTask;
    }
}
