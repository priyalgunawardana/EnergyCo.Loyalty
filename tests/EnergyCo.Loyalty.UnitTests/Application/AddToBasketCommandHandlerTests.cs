using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Baskets.Commands.AddToBasket;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Domain.Entities;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class AddToBasketCommandHandlerTests
{
    private readonly Mock<IBasketService> _basketService = new();
    private readonly AddToBasketCommandHandler _handler;

    public AddToBasketCommandHandlerTests()
    {
        _handler = new AddToBasketCommandHandler(_basketService.Object);
    }

    [Fact]
    public async Task Handle_ProductExists_CorrectPrice_ReturnsSuccess()
    {
        var basket = new Basket("cust-1");
        _basketService
            .Setup(s => s.AddItemAsync("cust-1", "PRD01", 1.2m, 3, default))
            .ReturnsAsync(Result.Success(basket));

        var result = await _handler.HandleAsync(new AddToBasketCommand("cust-1", "PRD01", 1.2m, 3), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsNotFoundFailure()
    {
        _basketService
            .Setup(s => s.AddItemAsync(It.IsAny<string>(), "PRD99", It.IsAny<decimal>(), It.IsAny<int>(), default))
            .ReturnsAsync(Result.Failure<Basket>("Product 'PRD99' not found.", ErrorKind.NotFound));

        var result = await _handler.HandleAsync(new AddToBasketCommand("cust-1", "PRD99", 1.2m, 1), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorKind.Should().Be(ErrorKind.NotFound);
    }

    [Fact]
    public async Task Handle_UnitPriceMismatch_ReturnsValidationFailure()
    {
        _basketService
            .Setup(s => s.AddItemAsync(It.IsAny<string>(), "PRD01", 2.0m, It.IsAny<int>(), default))
            .ReturnsAsync(Result.Failure<Basket>("Unit price 2.0 for 'PRD01' does not match the catalogue price 1.2.", ErrorKind.Validation));

        var result = await _handler.HandleAsync(new AddToBasketCommand("cust-1", "PRD01", 2.0m, 3), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorKind.Should().Be(ErrorKind.Validation);
    }
}
