using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;

namespace EnergyCo.Loyalty.Infrastructure.SeedData;

internal static class ProductSeed
{
    // The product list includes a mix of Fuel and Shop products, with unique codes to match with given data.
    public static IReadOnlyList<Product> GetProducts() =>
    [
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000001"), "PRD01", "Vortex 95",    new Money(1.2m), "Fuel"),
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000002"), "PRD02", "Vortex 98",    new Money(1.3m), "Fuel"),
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000003"), "PRD03", "Diesel",       new Money(1.1m), "Fuel"),
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000004"), "PRD04", "Twix 55g",     new Money(2.3m), "Shop"),
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000005"), "PRD05", "Mars 72g",     new Money(5.1m), "Shop"),
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000006"), "PRD06", "SNICKERS 72G", new Money(3.4m), "Shop"),
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000007"), "PRD07", "Bounty 3 63g", new Money(6.9m), "Shop"),
        new Product(ProductId.From("a1b2c3d4-0001-0001-0001-000000000008"), "PRD08", "Snickers 50g", new Money(4.0m), "Shop"),
    ];
}
