namespace EnergyCo.Loyalty.Domain.ValueObjects;

public record ProductId(Guid Value)
{
    public static ProductId New() => new(Guid.NewGuid());
    public static ProductId From(string value) => new(Guid.Parse(value));
    public override string ToString() => Value.ToString();
}
