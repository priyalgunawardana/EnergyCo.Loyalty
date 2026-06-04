using EnergyCo.Loyalty.Domain.ValueObjects;

namespace EnergyCo.Loyalty.Domain.Entities;

public class Product
{
    /// <summary>Internal stable identity (GUID). Used for all domain relationships.</summary>
    public ProductId Id { get; }

    /// <summary>Business/catalogue code (e.g. "PRD01"). Used in API requests and as in given request data.</summary>
    public string Code { get; }

    public string Name { get; }
    public Money Price { get; }
    public string Category { get; }

    public Product(ProductId id, string code, string name, Money price, string category)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(price);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        Id = id;
        Code = code;
        Name = name;
        Price = price;
        Category = category;
    }
}
