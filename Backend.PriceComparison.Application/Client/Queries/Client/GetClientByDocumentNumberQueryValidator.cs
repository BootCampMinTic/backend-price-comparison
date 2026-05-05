using FluentValidation;

namespace Backend.PriceComparison.Application.Client.Queries.Client
{
    public class GetClientByDocumentNumberQueryValidator : AbstractValidator<GetClientByDocumentNumberQuery>
    {
        public GetClientByDocumentNumberQueryValidator()
        {
            RuleFor(x => x.DocumentNumber)
                .NotNull().WithMessage("The document number cannot be null.");
        }
    }
}
