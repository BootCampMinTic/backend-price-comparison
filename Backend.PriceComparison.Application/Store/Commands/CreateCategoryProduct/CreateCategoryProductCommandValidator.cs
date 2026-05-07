using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;

public class CreateCategoryProductCommandValidator : AbstractValidator<CreateCategoryProductCommand>
{
    public CreateCategoryProductCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty();
    }
}
