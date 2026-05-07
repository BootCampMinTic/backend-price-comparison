using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.Store;

/// <summary>
/// Query to retrieve all stores with pagination.
/// </summary>
public record GetAllStoresQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<IEnumerable<StoreDto>, Error>>;
