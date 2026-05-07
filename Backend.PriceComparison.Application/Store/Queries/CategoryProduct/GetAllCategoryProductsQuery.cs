using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;

/// <summary>
/// Query to retrieve all category products.
/// </summary>
public record GetAllCategoryProductsQuery : IRequest<Result<IEnumerable<CategoryProductDto>, Error>>;
