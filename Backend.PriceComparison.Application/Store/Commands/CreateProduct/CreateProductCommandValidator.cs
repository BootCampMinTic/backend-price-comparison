using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.StoreId)
            .GreaterThan(0);

        RuleFor(x => x.CategoryProductId)
            .GreaterThan(0);
    }
}
