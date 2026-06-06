using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.UpdateCategoryProduct;

public sealed class UpdateCategoryProductCommandValidator : AbstractValidator<UpdateCategoryProductCommand>
{
    public UpdateCategoryProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(255);
    }
}
