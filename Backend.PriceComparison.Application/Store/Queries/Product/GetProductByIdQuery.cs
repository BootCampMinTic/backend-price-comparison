using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

/// <summary>
/// Query to retrieve a product by its identifier.
/// </summary>
public record GetProductByIdQuery(int Id) : IRequest<Result<ProductDto, Error>>;
