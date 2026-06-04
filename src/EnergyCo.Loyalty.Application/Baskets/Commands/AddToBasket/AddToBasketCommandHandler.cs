using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Abstractions.Messaging;
using EnergyCo.Loyalty.Application.Common;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.AddToBasket;

public sealed class AddToBasketCommandHandler(IBasketService basketService)
    : ICommandHandler<AddToBasketCommand, Result>
{
    public async Task<Result> HandleAsync(AddToBasketCommand command, CancellationToken cancellationToken = default)
    {
        var result = await basketService.AddItemAsync(
            command.CustomerId, command.ProductId, command.UnitPrice, command.Quantity, cancellationToken);

        return result.IsFailure ? Result.Failure(result.Error!, result.ErrorKind) : Result.Success();
    }
}
