using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Services;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class PointsCalculatorTests
{
    private readonly Mock<IPointsPromotionRepository> _pointsPromoRepo = new();
    private readonly PointsCalculator _calculator;

    private static readonly DateOnly _testDate = new(2020, 4, 3);

    public PointsCalculatorTests()
    {
        _calculator = new PointsCalculator(_pointsPromoRepo.Object);
        _pointsPromoRepo.Setup(r => r.GetActiveOnDate(It.IsAny<DateOnly>())).Returns([]);
    }

    [Fact]
    public void Calculate_NoActivePromotions_ReturnsZero()
    {
        var item = BuildItem("PRD01", 1.2m, "Fuel", 10);

        var points = _calculator.Calculate(item, _testDate);

        points.Should().Be(0);
    }

    [Fact]
    public void Calculate_CategoryDoesNotMatch_ReturnsZero()
    {
        var promo = new PointsPromotion("PP001", "Fuel Promo",
            new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 31), "Fuel", 3);
        _pointsPromoRepo.Setup(r => r.GetActiveOnDate(_testDate)).Returns([promo]);

        var item = BuildItem("PRD04", 2.3m, "Shop", 4); // Shop, not Fuel

        var points = _calculator.Calculate(item, _testDate);

        points.Should().Be(0);
    }

    [Fact]
    public void Calculate_MatchingCategoryPromo_AwardsPointsOnWholeUnitsOnly()
    {
        var promo = new PointsPromotion("PP002", "Fuel Promo",
            new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 31), "Fuel", 3);
        _pointsPromoRepo.Setup(r => r.GetActiveOnDate(_testDate)).Returns([promo]);

        var item = BuildItem("PRD03", 1.1m, "Fuel", 10);

        var points = _calculator.Calculate(item, _testDate);

        points.Should().Be(33);
    }

    [Fact]
    public void Calculate_AnyPromo_MatchesAllCategories()
    {
        var promo = new PointsPromotion("PP001", "New Year Promo",
            new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 31), "Any", 2);
        _pointsPromoRepo.Setup(r => r.GetActiveOnDate(_testDate)).Returns([promo]);

        var item = BuildItem("PRD04", 2.3m, "Shop", 4);

        var points = _calculator.Calculate(item, _testDate);

        points.Should().Be(18);
    }

    private static BasketItem BuildItem(string code, decimal price, string category, int qty) =>
        new(ProductId.New(), code, $"Product {code}", category, new Money(price), qty);
}
