using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;

public sealed class CreateCategoryProductCommandValidator : AbstractValidator<CreateCategoryProductCommand>
{
    public CreateCategoryProductCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(255);
    }
}
