using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

/// <summary>
/// Query to retrieve a category store by its identifier.
/// </summary>
public record GetCategoryStoreByIdQuery(int Id) : IRequest<Result<CategoryStoreDto, Error>>;
