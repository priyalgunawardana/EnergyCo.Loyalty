using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Services;

internal sealed class BasketService(
    IBasketRepository basketRepository,
    IPricingService pricingService) : IBasketService
{
    public async Task<Result<Basket>> AddItemAsync(
        string customerId, string productCode, decimal unitPrice, int quantity,
        CancellationToken cancellationToken = default)
    {
        var priceResult = await pricingService.ValidatePriceAsync(productCode, unitPrice, cancellationToken);
        if (priceResult.IsFailure)
            return Result.Failure<Basket>(priceResult.Error!, priceResult.ErrorKind);

        var basket = await basketRepository.GetByCustomerIdAsync(customerId, cancellationToken)
                     ?? new Basket(customerId);

        basket.AddItem(priceResult.Value!, quantity);
        await basketRepository.SaveAsync(basket, cancellationToken);

        return Result.Success(basket);
    }

    public Task<Basket?> FindAsync(string customerId, CancellationToken cancellationToken = default)
        => basketRepository.GetByCustomerIdAsync(customerId, cancellationToken);

    public Task RemoveAsync(string customerId, CancellationToken cancellationToken = default)
        => basketRepository.DeleteAsync(customerId, cancellationToken);
}
