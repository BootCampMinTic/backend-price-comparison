using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.RegisterPrice;

/// <summary>
/// Reglas de validación para el comando RegisterPriceCommand.
/// </summary>
public class RegisterPriceCommandValidator : AbstractValidator<RegisterPriceCommand>
{
    public RegisterPriceCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("El identificador del producto debe ser mayor a cero.");

        RuleFor(x => x.StoreId)
            .GreaterThan(0)
            .WithMessage("El identificador del supermercado debe ser mayor a cero.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("El precio debe ser mayor a cero.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("La fecha del registro no puede estar vacía.")
            .LessThanOrEqualTo(x => DateTime.UtcNow)
            .WithMessage("La fecha del registro no puede ser en el futuro.");
    }
}
