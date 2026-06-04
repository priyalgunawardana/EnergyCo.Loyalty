using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Exceptions;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.CheckoutBasket;

public sealed class CheckoutBasketCommandHandler(
    IBasketService basketService,
    ICheckoutService checkoutService) : ICommandHandler<CheckoutBasketCommand, Result<CheckoutSummaryDto>>
{
    public async Task<Result<CheckoutSummaryDto>> HandleAsync(
        CheckoutBasketCommand command, CancellationToken cancellationToken = default)
    {
        var basket = await basketService.FindAsync(command.CustomerId, cancellationToken);
        if (basket is null)
            throw new BasketNotFoundException(command.CustomerId);

        if (basket.IsEmpty)
            throw new BasketEmptyException($"Cannot checkout an empty basket for customer '{command.CustomerId}'.");

        var result = await checkoutService.ProcessAsync(basket, command.LoyaltyCard, command.TransactionDate, cancellationToken);
        if (result.IsFailure)
            return result;

        // Only remove the basket once we know the summary was built successfully.
        await basketService.RemoveAsync(command.CustomerId, cancellationToken);
        return result;
    }
}
