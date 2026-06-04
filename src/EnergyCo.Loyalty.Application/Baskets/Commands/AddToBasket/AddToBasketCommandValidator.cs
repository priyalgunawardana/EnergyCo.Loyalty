using FluentValidation;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.AddToBasket;

public sealed class AddToBasketCommandValidator : AbstractValidator<AddToBasketCommand>
{
    public AddToBasketCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("A customer ID is required.");
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("A product code is required.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than zero.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}
