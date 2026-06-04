using FluentValidation;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.CheckoutBasket;

public sealed class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.LoyaltyCard).NotEmpty().WithMessage("LoyaltyCard is required.");
    }
}
