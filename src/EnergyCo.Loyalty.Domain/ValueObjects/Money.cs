namespace EnergyCo.Loyalty.Domain.ValueObjects;

public record Money(decimal Amount, string Currency = Money.DefaultCurrency)
{
    public const string DefaultCurrency = "AUD";

    public static Money Zero(string currency = DefaultCurrency) => new(0m, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies.");
        return this with { Amount = Amount + other.Amount };
    }
    public Money Multiply(decimal factor) => this with { Amount = Amount * factor };

    public static Money operator +(Money a, Money b) => a.Add(b);
    public static Money operator *(Money a, decimal factor) => a.Multiply(factor);
}
