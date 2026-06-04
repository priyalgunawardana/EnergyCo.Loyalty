using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.Exceptions;

namespace EnergyCo.Loyalty.Application.Services;

/// <summary>Looks up a product and rejects any price that differs from the catalogue.</summary>
internal sealed class PricingService(IProductRepository productRepository) : IPricingService
{
    public async Task<Result<Product>> ValidatePriceAsync(
        string productCode, decimal submittedPrice, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByCodeAsync(productCode, cancellationToken);
        if (product is null)
            throw new ProductNotFoundException(productCode);

        if (submittedPrice != product.Price.Amount)
            return Result.Failure<Product>(
                $"Unit price {submittedPrice} for '{productCode}' does not match the catalogue price {product.Price.Amount}.",
                ErrorKind.Validation);

        return Result.Success(product);
    }
}
