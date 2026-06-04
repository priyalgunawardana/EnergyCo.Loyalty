namespace EnergyCo.Loyalty.Domain.Exceptions;

public class BasketEmptyException(string message) : Exception(message);

public class ProductNotFoundException(string productId)
    : Exception($"Product '{productId}' was not found.");

public class BasketNotFoundException(string customerId)
    : Exception($"Basket for customer '{customerId}' was not found.");
