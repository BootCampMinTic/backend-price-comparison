using FluentValidation;

namespace Backend.PriceComparison.Application.Client.Queries.Client
{
    public class GetClientByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
    {
        public GetClientByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("The Id cannot be null.")
                .GreaterThan(0).WithMessage("The Id must be greater than 0.");
        }
    }
}
