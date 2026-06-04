using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.AddToBasket;

public record AddToBasketCommand(string CustomerId, string ProductId, decimal UnitPrice, int Quantity) : ICommand<Result>;
