using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Application.Services;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class BasketServiceTests
{
    private readonly Mock<IBasketRepository> _basketRepo = new();
    private readonly Mock<IPricingService> _pricingService = new();
    private readonly BasketService _service;

    public BasketServiceTests()
    {
        _service = new BasketService(_basketRepo.Object, _pricingService.Object);
    }

    [Fact]
    public async Task AddItem_PriceValidationFails_ReturnsFailureWithoutSaving()
    {
        _pricingService
            .Setup(s => s.ValidatePriceAsync("PRD99", It.IsAny<decimal>(), default))
            .ReturnsAsync(Result.Failure<Product>("Product 'PRD99' not found.", ErrorKind.NotFound));

        var result = await _service.AddItemAsync("cust-1", "PRD99", 1.2m, 1);

        result.IsFailure.Should().BeTrue();
        result.ErrorKind.Should().Be(ErrorKind.NotFound);
        _basketRepo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), default), Times.Never);
    }

    [Fact]
    public async Task AddItem_NewCustomer_CreatesBasketAndSaves()
    {
        var product = new Product(ProductId.New(), "PRD01", "Vortex 95", new Money(1.2m), "Fuel");
        _pricingService
            .Setup(s => s.ValidatePriceAsync("PRD01", 1.2m, default))
            .ReturnsAsync(Result.Success(product));
        _basketRepo.Setup(r => r.GetByCustomerIdAsync("cust-1", default)).ReturnsAsync((Basket?)null);

        var result = await _service.AddItemAsync("cust-1", "PRD01", 1.2m, 3);

        result.IsSuccess.Should().BeTrue();
        _basketRepo.Verify(r => r.SaveAsync(It.Is<Basket>(b => b.CustomerId == "cust-1"), default), Times.Once);
    }

    [Fact]
    public async Task AddItem_ExistingBasket_AppendsItemAndSaves()
    {
        var product = new Product(ProductId.New(), "PRD01", "Vortex 95", new Money(1.2m), "Fuel");
        var existing = new Basket("cust-1");
        _pricingService
            .Setup(s => s.ValidatePriceAsync("PRD01", 1.2m, default))
            .ReturnsAsync(Result.Success(product));
        _basketRepo.Setup(r => r.GetByCustomerIdAsync("cust-1", default)).ReturnsAsync(existing);

        await _service.AddItemAsync("cust-1", "PRD01", 1.2m, 2);

        existing.Items.Should().HaveCount(1);
        existing.Items[0].Quantity.Should().Be(2);
        _basketRepo.Verify(r => r.SaveAsync(existing, default), Times.Once);
    }
}
