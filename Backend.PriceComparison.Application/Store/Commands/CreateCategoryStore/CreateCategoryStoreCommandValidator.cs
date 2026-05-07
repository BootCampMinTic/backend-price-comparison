using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;

public class CreateCategoryStoreCommandValidator : AbstractValidator<CreateCategoryStoreCommand>
{
    public CreateCategoryStoreCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty();
    }
}
