using EnergyCo.Loyalty.Domain.ValueObjects;

namespace EnergyCo.Loyalty.Domain.Entities;

public class BasketItem
{
    public ProductId ProductId { get; }
    public string ProductCode { get; }
    public string ProductName { get; }
    public string Category { get; }
    public Money UnitPrice { get; }
    public int Quantity { get; private set; }

    public Money LineTotal => UnitPrice * Quantity;

    public BasketItem(ProductId productId, string productCode, string productName, string category, Money unitPrice, int quantity)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentNullException.ThrowIfNull(unitPrice);
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive");

        ProductId = productId;
        ProductCode = productCode;
        ProductName = productName;
        Category = category;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Quantity += amount;
    }
}
