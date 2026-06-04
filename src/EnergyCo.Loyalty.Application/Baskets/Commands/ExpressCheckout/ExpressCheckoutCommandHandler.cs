using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.ExpressCheckout;

public sealed class ExpressCheckoutCommandHandler(
    IPricingService pricingService,
    ICheckoutService checkoutService) : ICommandHandler<ExpressCheckoutCommand, Result<CheckoutSummaryDto>>
{
    public async Task<Result<CheckoutSummaryDto>> HandleAsync(
        ExpressCheckoutCommand command,
        CancellationToken cancellationToken = default)
    {
        // Build a transient in-memory basket — express flow requires no prior basket state.
        var basket = new Basket(command.CustomerId);

        foreach (var item in command.Items)
        {
            var priceResult = await pricingService.ValidatePriceAsync(item.ProductId, item.UnitPrice, cancellationToken);
            if (priceResult.IsFailure)
                return Result.Failure<CheckoutSummaryDto>(priceResult.Error!, priceResult.ErrorKind);

            basket.AddItem(priceResult.Value!, item.Quantity);
        }

        return await checkoutService.ProcessAsync(basket, command.LoyaltyCard, command.TransactionDate, cancellationToken);
    }
}
