using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
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
