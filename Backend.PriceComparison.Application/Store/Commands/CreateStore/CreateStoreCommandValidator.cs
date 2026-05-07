using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateStore;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Address)
            .NotEmpty();

        RuleFor(x => x.CategoryStoreId)
            .GreaterThan(0);
    }
}
