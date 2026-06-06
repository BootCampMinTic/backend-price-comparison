using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.UpdateCategoryStore;

public sealed class UpdateCategoryStoreCommandValidator : AbstractValidator<UpdateCategoryStoreCommand>
{
    public UpdateCategoryStoreCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(255);
    }
}
