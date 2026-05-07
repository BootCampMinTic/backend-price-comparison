using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

/// <summary>
/// Query to retrieve all products with pagination.
/// </summary>
public record GetAllProductsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<IEnumerable<ProductDto>, Error>>;
