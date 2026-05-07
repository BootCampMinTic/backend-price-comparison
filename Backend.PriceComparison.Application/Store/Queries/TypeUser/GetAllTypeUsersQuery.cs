using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.TypeUser;

/// <summary>
/// Query to retrieve all type users.
/// </summary>
public record GetAllTypeUsersQuery : IRequest<Result<IEnumerable<TypeUserDto>, Error>>;
