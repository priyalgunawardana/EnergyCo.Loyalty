using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;

namespace EnergyCo.Loyalty.Application.Services;

internal sealed class CheckoutService(
    IDiscountCalculator discountCalculator,
    IPointsCalculator pointsCalculator) : ICheckoutService
{
    public Task<Result<CheckoutSummaryDto>> ProcessAsync(
        Basket basket, string loyaltyCard, DateOnly transactionDate,
        CancellationToken cancellationToken = default)
    {
        var lineResults = basket.Items
            .Select(item =>
            {
                var lineTotal = item.LineTotal.Amount;
                var discount  = discountCalculator.Calculate(item, transactionDate);
                var points    = pointsCalculator.Calculate(item, transactionDate);
                return (item, lineTotal, discount, points);
            })
            .ToList();

        var subtotal      = Math.Round(lineResults.Sum(x => x.lineTotal), 2);
        var totalDiscount = Math.Round(lineResults.Sum(x => x.discount), 2);
        var grandTotal    = Math.Round(subtotal - totalDiscount, 2);
        var rewardPoints  = lineResults.Sum(x => x.points);

        var dto = new CheckoutSummaryDto(
            basket.CustomerId,
            loyaltyCard,
            transactionDate.ToString("dd-MMM-yyyy"),
            subtotal,
            totalDiscount,
            grandTotal,
            basket.Subtotal.Currency,
            rewardPoints,
            lineResults.Select(x => new BasketItemDto(
                x.item.ProductCode,
                x.item.ProductName,
                x.item.UnitPrice.Amount,
                x.item.Quantity,
                Math.Round(x.lineTotal, 2))).ToList());

        return Task.FromResult(Result.Success(dto));
    }
}
