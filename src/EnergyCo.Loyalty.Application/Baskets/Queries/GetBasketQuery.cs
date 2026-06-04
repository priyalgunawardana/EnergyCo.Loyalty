using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Baskets.Queries;

public record GetBasketQuery(string CustomerId) : IQuery<Result<BasketDto>>;

public record BasketDto(
    string CustomerId,
    decimal Subtotal,
    IReadOnlyList<BasketItemDto> Items);
