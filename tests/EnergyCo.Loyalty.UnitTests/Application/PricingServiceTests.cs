using EnergyCo.Loyalty.Application.Abstractions;
using EnergyCo.Loyalty.Application.Common;
using EnergyCo.Loyalty.Application.Services;
using EnergyCo.Loyalty.Domain.Entities;
using EnergyCo.Loyalty.Domain.Exceptions;
using EnergyCo.Loyalty.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class PricingServiceTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly PricingService _service;

    public PricingServiceTests()
    {
        _service = new PricingService(_productRepo.Object);
    }

    [Fact]
    public async Task ValidatePrice_ProductNotFound_ThrowsProductNotFoundException()
    {
        _productRepo.Setup(r => r.GetByCodeAsync("PRD99", default)).ReturnsAsync((Product?)null);

        var act = () => _service.ValidatePriceAsync("PRD99", 1.2m);

        await act.Should().ThrowAsync<ProductNotFoundException>()
            .WithMessage("*PRD99*");
    }

    [Fact]
    public async Task ValidatePrice_PriceMismatch_ReturnsValidationFailure()
    {
        var product = new Product(ProductId.New(), "PRD01", "Vortex 95", new Money(1.2m), "Fuel");
        _productRepo.Setup(r => r.GetByCodeAsync("PRD01", default)).ReturnsAsync(product);

        var result = await _service.ValidatePriceAsync("PRD01", 2.0m);

        result.IsFailure.Should().BeTrue();
        result.ErrorKind.Should().Be(ErrorKind.Validation);
        result.Error.Should().Contain("2").And.Contain("1.2");
    }

    [Fact]
    public async Task ValidatePrice_CorrectPrice_ReturnsProductInResult()
    {
        var product = new Product(ProductId.New(), "PRD01", "Vortex 95", new Money(1.2m), "Fuel");
        _productRepo.Setup(r => r.GetByCodeAsync("PRD01", default)).ReturnsAsync(product);

        var result = await _service.ValidatePriceAsync("PRD01", 1.2m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(product);
    }
}
