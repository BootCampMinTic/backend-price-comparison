using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.Sale;

/// <summary>
/// Query to retrieve a sale by its identifier.
/// </summary>
public record GetSaleByIdQuery(int Id) : IRequest<Result<SaleDto, Error>>;
