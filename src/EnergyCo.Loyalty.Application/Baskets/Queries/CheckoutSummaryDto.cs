namespace EnergyCo.Loyalty.Application.Baskets.Queries;

public record CheckoutSummaryDto(
    string CustomerId,
    string LoyaltyCard,
    string TransactionDate,
    decimal Subtotal,
    decimal Discount,
    decimal Total,
    string Currency,
    int RewardPoints,
    IReadOnlyList<BasketItemDto> Items);

public record BasketItemDto(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
