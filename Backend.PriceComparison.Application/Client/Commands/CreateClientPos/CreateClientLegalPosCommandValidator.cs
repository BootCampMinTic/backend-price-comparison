using FluentValidation;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

namespace Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

public class CreateClientLegalPosCommandValidator : AbstractValidator<CreateClientLegalPosCommand>
{
    public CreateClientLegalPosCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("The company name is required.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("The document number is required.");

        RuleFor(x => x.ElectronicInvoiceEmail)
            .NotEmpty().WithMessage("The email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.DocumentTypeId)
            .GreaterThan(0).WithMessage("A valid document type is required.");
    }
}
