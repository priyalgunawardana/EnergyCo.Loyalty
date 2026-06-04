using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.CheckoutBasket;

public record CheckoutBasketCommand(
    string CustomerId,
    string LoyaltyCard,
    DateOnly TransactionDate) : ICommand<Result<CheckoutSummaryDto>>;
