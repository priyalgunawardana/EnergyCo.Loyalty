using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Services;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class CheckoutServiceTests
{
    private readonly Mock<IDiscountCalculator> _discountCalculator = new();
    private readonly Mock<IPointsCalculator> _pointsCalculator = new();
    private readonly CheckoutService _service;

    private static readonly DateOnly _testDate = new(2020, 4, 3);

    public CheckoutServiceTests()
    {
        _service = new CheckoutService(_discountCalculator.Object, _pointsCalculator.Object);
        _discountCalculator.Setup(c => c.Calculate(It.IsAny<BasketItem>(), It.IsAny<DateOnly>())).Returns(0m);
        _pointsCalculator.Setup(c => c.Calculate(It.IsAny<BasketItem>(), It.IsAny<DateOnly>())).Returns(0);
    }

    [Fact]
    public async Task Process_NoDiscountOrPoints_ReturnsTotalsWithZeroDiscountAndZeroPoints()
    {
        var basket = BuildBasket(("PRD01", "Vortex 95", 1.2m, "Fuel", 2));

        var result = await _service.ProcessAsync(basket, "CTX0000001", _testDate);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Subtotal.Should().Be(2.4m);
        result.Value.Discount.Should().Be(0m);
        result.Value.Total.Should().Be(2.4m);
        result.Value.RewardPoints.Should().Be(0);
    }

    [Fact]
    public async Task Process_LoyaltyCardAndTransactionDate_PresentInResponse()
    {
        var basket = BuildBasket(("PRD01", "Vortex 95", 1.2m, "Fuel", 1));

        var result = await _service.ProcessAsync(basket, "CTX0000001", _testDate);

        result.Value!.LoyaltyCard.Should().Be("CTX0000001");
        result.Value.TransactionDate.Should().Be("03-Apr-2020");
    }

    [Fact]
    public async Task Process_DiscountApplied_SubtractedFromTotal()
    {
        var basket = BuildBasket(("PRD02", "Vortex 98", 1.3m, "Fuel", 10));

        _discountCalculator
            .Setup(c => c.Calculate(It.Is<BasketItem>(i => i.ProductCode == "PRD02"), _testDate))
            .Returns(2.60m);

        var result = await _service.ProcessAsync(basket, "CTX0000001", _testDate);

        result.Value!.Subtotal.Should().Be(13m);
        result.Value.Discount.Should().Be(2.60m);
        result.Value.Total.Should().Be(10.40m);
    }

    [Fact]
    public async Task Process_PointsReturned_SummedAcrossLines()
    {
        var basket = BuildBasket(
            ("PRD01", "Vortex 95", 1.2m, "Fuel", 10),
            ("PRD03", "Diesel",    1.1m, "Fuel", 10));

        _pointsCalculator
            .Setup(c => c.Calculate(It.Is<BasketItem>(i => i.ProductCode == "PRD01"), _testDate))
            .Returns(12);
        _pointsCalculator
            .Setup(c => c.Calculate(It.Is<BasketItem>(i => i.ProductCode == "PRD03"), _testDate))
            .Returns(33);

        var result = await _service.ProcessAsync(basket, "CTX0000001", _testDate);

        result.Value!.RewardPoints.Should().Be(45);
    }

    [Fact]
    public async Task Process_CallsCalculatorsOncePerLineItem()
    {
        var basket = BuildBasket(
            ("PRD01", "Vortex 95", 1.2m, "Fuel", 2),
            ("PRD02", "Vortex 98", 1.3m, "Fuel", 3));

        await _service.ProcessAsync(basket, "CTX0000001", _testDate);

        _discountCalculator.Verify(c => c.Calculate(It.IsAny<BasketItem>(), _testDate), Times.Exactly(2));
        _pointsCalculator.Verify(c => c.Calculate(It.IsAny<BasketItem>(), _testDate), Times.Exactly(2));
    }

    // ── Helpers -------------------------------------

    private static Basket BuildBasket(params (string code, string name, decimal price, string category, int qty)[] items)
    {
        var basket = new Basket("cust-1");
        foreach (var (code, name, price, category, qty) in items)
        {
            basket.AddItem(new Product(ProductId.New(), code, name, new Money(price), category), qty);
        }
        return basket;
    }
}
