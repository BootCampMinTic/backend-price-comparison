using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.StoreId)
            .GreaterThan(0);

        RuleFor(x => x.StateId)
            .GreaterThan(0);

        RuleFor(x => x.ProductIds)
            .NotEmpty();
    }
}
