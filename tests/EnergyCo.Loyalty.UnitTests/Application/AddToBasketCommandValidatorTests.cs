using FluentValidation;
using FluentValidation.TestHelper;
using EnergyCo.Loyalty.Application.Baskets.Commands.AddToBasket;

namespace EnergyCo.Loyalty.UnitTests.Application;

public class AddToBasketCommandValidatorTests
{
    private readonly IValidator<AddToBasketCommand> _validator = new AddToBasketCommandValidator();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var cmd = new AddToBasketCommand("cust-1", "PRD01", 1.2m, 2);
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyCustomerId_FailsValidation()
    {
        var cmd = new AddToBasketCommand("", "PRD01", 1.2m, 2);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void Validate_EmptyProductId_FailsValidation()
    {
        var cmd = new AddToBasketCommand("cust-1", "", 1.2m, 2);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1.5)]
    public void Validate_ZeroOrNegativeUnitPrice_FailsValidation(decimal price)
    {
        var cmd = new AddToBasketCommand("cust-1", "PRD01", price, 2);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ZeroOrNegativeQuantity_FailsValidation(int quantity)
    {
        var cmd = new AddToBasketCommand("cust-1", "PRD01", 1.2m, quantity);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}
