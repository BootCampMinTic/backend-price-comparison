using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

/// <summary>
/// Query to retrieve all category stores.
/// </summary>
public record GetAllCategoryStoresQuery : IRequest<Result<IEnumerable<CategoryStoreDto>, Error>>;
