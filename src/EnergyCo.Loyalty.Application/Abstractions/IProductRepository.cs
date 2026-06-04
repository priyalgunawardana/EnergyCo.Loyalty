using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;

namespace EnergyCo.Loyalty.Application.Abstractions;

/// <summary>Repository for product data.</summary>
public interface IProductRepository
{
    /// <summary>Looks up a product by its internal GUID identity.</summary>
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default);

    /// <summary>Looks up a product by its external business code (e.g. "PRD01").</summary>
    Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
}
