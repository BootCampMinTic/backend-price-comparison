using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;

public sealed class CreateCategoryStoreCommandValidator : AbstractValidator<CreateCategoryStoreCommand>
{
    public CreateCategoryStoreCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(255);
    }
}
