using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;

public class CreateCategoryProductCommandValidator : AbstractValidator<CreateCategoryProductCommand>
{
    public CreateCategoryProductCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripcion de la categoria es obligatoria.")
            .MaximumLength(100).WithMessage("La descripcion no puede superar los 100 caracteres.")
            .MinimumLength(3).WithMessage("La descripcion debe tener al menos 3 caracteres.");
    }
}
