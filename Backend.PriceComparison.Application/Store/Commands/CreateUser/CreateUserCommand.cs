using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Commands.CreateUser;

public record CreateUserCommand(
    string Name,
    string Password,
    int TypeUserId
) : IRequest<Result<VoidResult, Error>>;
