using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.User;

/// <summary>
/// Query to retrieve all users.
/// </summary>
public record GetAllUsersQuery : IRequest<Result<IEnumerable<UserDto>, Error>>;
