using System.Globalization;
using EnergyCo.Loyalty.Api.Contracts;
using EnergyCo.Loyalty.Application.Baskets.Queries;

namespace EnergyCo.Loyalty.Api.Mappings;

internal static class ApiResponseMapper
{
    internal const string TransactionDateFormat = "dd-MMM-yyyy";
    internal const string TransactionDateExample = "03-Apr-2020";

    internal static bool TryParseTransactionDate(string value, out DateOnly result) =>
        DateOnly.TryParseExact(
            value,
            TransactionDateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result);

    internal static BasketResponse ToBasketResponse(BasketDto dto)
    {
        var items = dto.Items
            .Select(i => new BasketItemResponse(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal))
            .ToList();

        return new BasketResponse(dto.CustomerId, dto.Subtotal, items);
    }

    internal static CheckoutResponse ToCheckoutResponse(CheckoutSummaryDto dto) =>
        new(dto.CustomerId,
            dto.LoyaltyCard,
            dto.TransactionDate,
            dto.Subtotal,
            dto.Discount,
            dto.Total,
            dto.RewardPoints);
}
