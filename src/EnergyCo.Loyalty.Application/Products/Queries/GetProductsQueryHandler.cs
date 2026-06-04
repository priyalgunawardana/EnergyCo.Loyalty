using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Products.Queries;

internal sealed class GetProductsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    public async Task<Result<IReadOnlyList<ProductDto>>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);

        var dtos = products.Select(p => new ProductDto(
            p.Id.ToString(),
            p.Code,
            p.Name,
            p.Price.Amount,
            p.Price.Currency,
            p.Category)).ToList();

        return Result.Success<IReadOnlyList<ProductDto>>(dtos);
    }
}
