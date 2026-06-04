using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Baskets.Queries;

internal sealed class GetBasketQueryHandler(IBasketRepository basketRepository)
    : IQueryHandler<GetBasketQuery, Result<BasketDto>>
{
    public async Task<Result<BasketDto>> HandleAsync(GetBasketQuery query, CancellationToken cancellationToken = default)
    {
        var basket = await basketRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);
        if (basket is null)
            return Result.Failure<BasketDto>($"No basket found for customer '{query.CustomerId}'.");

        var dto = new BasketDto(
            basket.CustomerId,
            basket.Subtotal.Amount,
            basket.Items.Select(i => new BasketItemDto(
                i.ProductId.ToString(),
                i.ProductName,
                i.UnitPrice.Amount,
                i.Quantity,
                i.LineTotal.Amount)).ToList());

        return Result.Success(dto);
    }
}
