using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Services;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class DiscountCalculatorTests
{
    private readonly Mock<IDiscountPromotionRepository> _discountPromoRepo = new();
    private readonly DiscountCalculator _calculator;

    private static readonly DateOnly _testDate = new(2020, 4, 3);

    public DiscountCalculatorTests()
    {
        _calculator = new DiscountCalculator(_discountPromoRepo.Object);
        _discountPromoRepo.Setup(r => r.GetActiveOnDate(It.IsAny<DateOnly>())).Returns([]);
    }

    [Fact]
    public void Calculate_NoActivePromotions_ReturnsZero()
    {
        var item = BuildItem("PRD01", 1.2m, "Fuel", 10);

        var discount = _calculator.Calculate(item, _testDate);

        discount.Should().Be(0m);
    }

    [Fact]
    public void Calculate_ProductNotEligible_ReturnsZero()
    {
        var promo = new DiscountPromotion("DP001", "Fuel Discount",
            new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 31), 20m, ["PRD02"]);
        _discountPromoRepo.Setup(r => r.GetActiveOnDate(_testDate)).Returns([promo]);

        var item = BuildItem("PRD01", 1.2m, "Fuel", 10); // PRD01 not in eligible list

        var discount = _calculator.Calculate(item, _testDate);

        discount.Should().Be(0m);
    }

    [Fact]
    public void Calculate_EligibleProduct_Returns20PercentOfLineTotal()
    {
        var promo = new DiscountPromotion("DP001", "Fuel Discount",
            new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 31), 20m, ["PRD02"]);
        _discountPromoRepo.Setup(r => r.GetActiveOnDate(_testDate)).Returns([promo]);

        var item = BuildItem("PRD02", 1.3m, "Fuel", 10);

        var discount = _calculator.Calculate(item, _testDate);

        discount.Should().Be(2.60m);
    }

    private static BasketItem BuildItem(string code, decimal price, string category, int qty) =>
        new(ProductId.New(), code, $"Product {code}", category, new Money(price), qty);
}
