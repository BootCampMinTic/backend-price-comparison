using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.State;

/// <summary>
/// Query to retrieve all states.
/// </summary>
public record GetAllStatesQuery : IRequest<Result<IEnumerable<StateDto>, Error>>;
