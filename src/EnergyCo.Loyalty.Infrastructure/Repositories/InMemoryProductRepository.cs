using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;
using EnergyCo.Loyalty.Infrastructure.SeedData;

namespace EnergyCo.Loyalty.Infrastructure.Repositories;

internal sealed class InMemoryProductRepository : IProductRepository
{
    private readonly IReadOnlyDictionary<ProductId, Product> _byId;
    private readonly IReadOnlyDictionary<string, Product> _byCode;

    public InMemoryProductRepository()
    {
        var products = ProductSeed.GetProducts();
        _byId   = products.ToDictionary(p => p.Id);
        _byCode = products.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);
    }

    public Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_byId.TryGetValue(id, out var p) ? p : null);

    public Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => Task.FromResult(_byCode.TryGetValue(code, out var p) ? p : null);

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Product>>(_byId.Values.ToList());
}
