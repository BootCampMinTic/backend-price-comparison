using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;

/// <summary>
/// Query to retrieve a category product by its identifier.
/// </summary>
public record GetCategoryProductByIdQuery(int Id) : IRequest<Result<CategoryProductDto, Error>>;
