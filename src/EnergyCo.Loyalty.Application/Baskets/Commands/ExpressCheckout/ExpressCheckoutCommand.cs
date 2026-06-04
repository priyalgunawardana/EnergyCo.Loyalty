using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.ExpressCheckout;

public record ExpressCheckoutItem(string ProductId, decimal UnitPrice, int Quantity);

public record ExpressCheckoutCommand(
    string CustomerId,
    string LoyaltyCard,
    DateOnly TransactionDate,
    IReadOnlyList<ExpressCheckoutItem> Items) : ICommand<Result<CheckoutSummaryDto>>;
