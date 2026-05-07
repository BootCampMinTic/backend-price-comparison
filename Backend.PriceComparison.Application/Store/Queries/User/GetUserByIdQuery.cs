using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.User;

/// <summary>
/// Query to retrieve a user by its identifier.
/// </summary>
public record GetUserByIdQuery(int Id) : IRequest<Result<UserDto, Error>>;
