using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Products.Queries;

public record GetProductsQuery : IQuery<Result<IReadOnlyList<ProductDto>>>;

public record ProductDto(string Id, string Code, string Name, decimal Price, string Currency, string Category);
