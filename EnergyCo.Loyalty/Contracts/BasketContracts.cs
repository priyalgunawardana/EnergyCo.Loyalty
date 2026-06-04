namespace EnergyCo.Loyalty.Api.Contracts;

public record AddToBasketRequest(string ProductId, decimal UnitPrice, int Quantity);

public record CheckoutRequest(string LoyaltyCard, string TransactionDate);

public record ExpressCheckoutItemRequest(string ProductId, decimal UnitPrice, int Quantity);

public record ExpressCheckoutRequest(
    string LoyaltyCard,
    string TransactionDate,
    IReadOnlyList<ExpressCheckoutItemRequest> Items);

public record CheckoutResponse(
    string CustomerId,
    string LoyaltyCard,
    string TransactionDate,
    decimal TotalAmount,
    decimal DiscountApplied,
    decimal GrandTotal,
    int PointsEarned);
public record BasketItemResponse(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public record BasketResponse(
    string CustomerId,
    decimal Subtotal,
    IReadOnlyList<BasketItemResponse> Items);
