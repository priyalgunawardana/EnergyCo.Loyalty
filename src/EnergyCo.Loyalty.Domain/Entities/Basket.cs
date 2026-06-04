using EnergyCo.Loyalty.Domain.ValueObjects;

namespace EnergyCo.Loyalty.Domain.Entities;

public class Basket
{
    private readonly List<BasketItem> _items = [];

    public Guid Id { get; } = Guid.NewGuid();
    public string CustomerId { get; }
    public IReadOnlyList<BasketItem> Items => _items.AsReadOnly();
    public bool IsEmpty => _items.Count == 0;
    public Money Subtotal => _items.Aggregate(Money.Zero(), (acc, item) => acc + item.LineTotal);

    public Basket(string customerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        CustomerId = customerId;
    }

    public void AddItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing is not null)
            existing.IncreaseQuantity(quantity);
        else
            _items.Add(new BasketItem(product.Id, product.Code, product.Name, product.Category, product.Price, quantity));
    }
}
