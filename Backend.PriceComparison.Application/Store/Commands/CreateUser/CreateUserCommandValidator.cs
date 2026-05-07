using FluentValidation;

namespace Backend.PriceComparison.Application.Store.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.TypeUserId)
            .GreaterThan(0);
    }
}
