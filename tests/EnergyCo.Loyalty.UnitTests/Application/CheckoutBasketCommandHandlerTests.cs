using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Baskets.Commands.CheckoutBasket;
using EnergyCo.Loyalty.Application.Baskets.Queries;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.Exceptions;
using EnergyCo.Loyalty.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class CheckoutBasketCommandHandlerTests
{
    private readonly Mock<IBasketService> _basketService = new();
    private readonly Mock<ICheckoutService> _checkoutService = new();
    private readonly CheckoutBasketCommandHandler _handler;

    private static readonly DateOnly _testDate = new(2020, 4, 3);

    private static CheckoutBasketCommand MakeCommand(string customerId = "cust-1") =>
        new(customerId, "CTX0000001", _testDate);

    private static CheckoutSummaryDto MakeDto(decimal subtotal = 2.4m, decimal discount = 0m, int points = 0) =>
        new("cust-1", "CTX0000001", "03-Apr-2020", subtotal, discount, subtotal - discount, "AUD", points, []);

    public CheckoutBasketCommandHandlerTests()
    {
        _handler = new CheckoutBasketCommandHandler(_basketService.Object, _checkoutService.Object);
    }

    [Fact]
    public async Task Handle_BasketNotFound_ThrowsBasketNotFoundException()
    {
        _basketService.Setup(s => s.FindAsync("cust-1", default)).ReturnsAsync((Basket?)null);

        var act = () => _handler.HandleAsync(MakeCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<BasketNotFoundException>()
            .WithMessage("*cust-1*");
    }

    [Fact]
    public async Task Handle_EmptyBasket_ThrowsBasketEmptyException()
    {
        _basketService.Setup(s => s.FindAsync("cust-1", default)).ReturnsAsync(new Basket("cust-1"));

        var act = () => _handler.HandleAsync(MakeCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<BasketEmptyException>()
            .WithMessage("*cust-1*");
    }

    [Fact]
    public async Task Handle_ValidBasket_ReturnsSuccessAndDeletesBasket()
    {
        var basket = new Basket("cust-1");
        basket.AddItem(new Product(ProductId.New(), "PRD01", "Vortex 95", new Money(1.2m), "Fuel"), 2);
        _basketService.Setup(s => s.FindAsync("cust-1", default)).ReturnsAsync(basket);
        _checkoutService.Setup(s => s.ProcessAsync(basket, "CTX0000001", _testDate, default))
            .ReturnsAsync(Result.Success(MakeDto()));

        var result = await _handler.HandleAsync(MakeCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _basketService.Verify(s => s.RemoveAsync("cust-1", default), Times.Once);
    }

    [Fact]
    public async Task Handle_CheckoutServiceFails_BasketIsNotDeleted()
    {
        var basket = new Basket("cust-1");
        basket.AddItem(new Product(ProductId.New(), "PRD01", "Vortex 95", new Money(1.2m), "Fuel"), 1);
        _basketService.Setup(s => s.FindAsync("cust-1", default)).ReturnsAsync(basket);
        _checkoutService.Setup(s => s.ProcessAsync(basket, It.IsAny<string>(), It.IsAny<DateOnly>(), default))
            .ReturnsAsync(Result.Failure<CheckoutSummaryDto>("Pricing error.", ErrorKind.Validation));

        var result = await _handler.HandleAsync(MakeCommand(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _basketService.Verify(s => s.RemoveAsync(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_PassesLoyaltyCardAndTransactionDateToCheckoutService()
    {
        var basket = new Basket("cust-1");
        basket.AddItem(new Product(ProductId.New(), "PRD01", "Vortex 95", new Money(1.2m), "Fuel"), 1);
        _basketService.Setup(s => s.FindAsync("cust-1", default)).ReturnsAsync(basket);
        _checkoutService.Setup(s => s.ProcessAsync(basket, "CTX0000001", _testDate, default))
            .ReturnsAsync(Result.Success(MakeDto()));

        await _handler.HandleAsync(MakeCommand(), CancellationToken.None);

        _checkoutService.Verify(s => s.ProcessAsync(basket, "CTX0000001", _testDate, default), Times.Once);
    }
}
