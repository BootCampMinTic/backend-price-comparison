using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateSale;

public sealed class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Sale date is required");

        RuleFor(x => x.Total)
            .GreaterThan(0).WithMessage("Total must be greater than zero");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than zero");

        RuleFor(x => x.StateId)
            .GreaterThan(0).WithMessage("StateId must be greater than zero");
    }
}
