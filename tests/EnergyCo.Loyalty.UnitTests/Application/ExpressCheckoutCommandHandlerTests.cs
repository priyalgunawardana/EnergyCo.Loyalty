using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Baskets.Commands.ExpressCheckout;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class ExpressCheckoutCommandHandlerTests
{
    private readonly Mock<IPricingService> _pricingService = new();
    private readonly Mock<ICheckoutService> _checkoutService = new();
    private readonly ExpressCheckoutCommandHandler _handler;

    private static readonly DateOnly _testDate = new(2020, 4, 3);

    private static Product MakeProduct(string code = "PRD01", string name = "Vortex 95", decimal price = 1.20m, string category = "Fuel") =>
        new(ProductId.New(), code, name, new Money(price), category);

    private static CheckoutSummaryDto MakeDto(decimal subtotal = 2.4m, decimal discount = 0m, int points = 0) =>
        new("cust-1", "CTX0000001", "03-Apr-2020", subtotal, discount, subtotal - discount, "AUD", points, []);

    private static ExpressCheckoutCommand MakeCommand(IReadOnlyList<ExpressCheckoutItem>? items = null) =>
        new("cust-1", "CTX0000001", _testDate,
            items ?? [new ExpressCheckoutItem("PRD01", 1.20m, 2)]);

    public ExpressCheckoutCommandHandlerTests()
    {
        _handler = new ExpressCheckoutCommandHandler(_pricingService.Object, _checkoutService.Object);
    }

    [Fact]
    public async Task Handle_ValidItems_CallsCheckoutServiceAndReturnsSuccess()
    {
        var product = MakeProduct();
        _pricingService.Setup(s => s.ValidatePriceAsync("PRD01", 1.20m, default))
            .ReturnsAsync(Result.Success(product));
        _checkoutService.Setup(s => s.ProcessAsync(It.IsAny<Basket>(), "CTX0000001", _testDate, default))
            .ReturnsAsync(Result.Success(MakeDto()));

        var result = await _handler.HandleAsync(MakeCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _checkoutService.Verify(s => s.ProcessAsync(It.IsAny<Basket>(), "CTX0000001", _testDate, default), Times.Once);
    }

    [Fact]
    public async Task Handle_UnknownProduct_ReturnsNotFoundFailure()
    {
        _pricingService.Setup(s => s.ValidatePriceAsync("PRD99", It.IsAny<decimal>(), default))
            .ReturnsAsync(Result.Failure<Product>("Product 'PRD99' not found.", ErrorKind.NotFound));

        var result = await _handler.HandleAsync(
            MakeCommand([new ExpressCheckoutItem("PRD99", 1.00m, 1)]),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorKind.Should().Be(ErrorKind.NotFound);
    }

    [Fact]
    public async Task Handle_WrongPrice_ReturnsValidationFailure()
    {
        _pricingService.Setup(s => s.ValidatePriceAsync("PRD01", 9.99m, default))
            .ReturnsAsync(Result.Failure<Product>("Price mismatch for 'PRD01'.", ErrorKind.Validation));

        var result = await _handler.HandleAsync(
            MakeCommand([new ExpressCheckoutItem("PRD01", 9.99m, 5)]),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorKind.Should().Be(ErrorKind.Validation);
    }

    [Fact]
    public async Task Handle_FirstItemFailsPricing_ShortCircuitsAndDoesNotCallCheckout()
    {
        _pricingService.Setup(s => s.ValidatePriceAsync("PRD99", It.IsAny<decimal>(), default))
            .ReturnsAsync(Result.Failure<Product>("Product not found.", ErrorKind.NotFound));

        var items = new[]
        {
            new ExpressCheckoutItem("PRD99", 1.00m, 1),
            new ExpressCheckoutItem("PRD01", 1.20m, 2)
        };

        await _handler.HandleAsync(MakeCommand(items), CancellationToken.None);

        _checkoutService.Verify(s => s.ProcessAsync(It.IsAny<Basket>(), It.IsAny<string>(), It.IsAny<DateOnly>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleValidItems_AddsAllToBasketBeforeCheckout()
    {
        var product1 = MakeProduct("PRD01", "Vortex 95", 1.20m, "Fuel");
        var product2 = MakeProduct("PRD04", "Twix 55g", 2.30m, "Shop");
        _pricingService.Setup(s => s.ValidatePriceAsync("PRD01", 1.20m, default)).ReturnsAsync(Result.Success(product1));
        _pricingService.Setup(s => s.ValidatePriceAsync("PRD04", 2.30m, default)).ReturnsAsync(Result.Success(product2));
        _checkoutService.Setup(s => s.ProcessAsync(It.Is<Basket>(b => b.Items.Count == 2), "CTX0000001", _testDate, default))
            .ReturnsAsync(Result.Success(MakeDto(19.30m)));

        var items = new[]
        {
            new ExpressCheckoutItem("PRD01", 1.20m, 10),
            new ExpressCheckoutItem("PRD04", 2.30m, 3)
        };

        var result = await _handler.HandleAsync(MakeCommand(items), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidItems_CheckoutServiceCalledExactlyOnce()
    {
        var product = MakeProduct();
        _pricingService.Setup(s => s.ValidatePriceAsync("PRD01", 1.20m, default)).ReturnsAsync(Result.Success(product));
        _checkoutService.Setup(s => s.ProcessAsync(It.IsAny<Basket>(), It.IsAny<string>(), It.IsAny<DateOnly>(), default))
            .ReturnsAsync(Result.Success(MakeDto()));

        await _handler.HandleAsync(MakeCommand(), CancellationToken.None);

        _checkoutService.Verify(s => s.ProcessAsync(It.IsAny<Basket>(), It.IsAny<string>(), It.IsAny<DateOnly>(), default), Times.Once);
    }
}
