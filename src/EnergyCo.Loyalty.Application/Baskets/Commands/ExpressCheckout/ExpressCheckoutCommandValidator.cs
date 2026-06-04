using FluentValidation;

namespace EnergyCo.Loyalty.Application.Baskets.Commands.ExpressCheckout;

public sealed class ExpressCheckoutCommandValidator : AbstractValidator<ExpressCheckoutCommand>
{
    public ExpressCheckoutCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.LoyaltyCard).NotEmpty().WithMessage("LoyaltyCard is required.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one item is required.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("ProductId is required.");
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            item.RuleFor(i => i.UnitPrice).GreaterThan(0).WithMessage("UnitPrice must be greater than zero.");
        });
    }
}
