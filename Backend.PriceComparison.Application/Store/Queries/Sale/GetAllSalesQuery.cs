using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;

namespace Backend.PriceComparison.Application.Store.Queries.Sale;

/// <summary>
/// Query to retrieve all sales with pagination.
/// </summary>
public record GetAllSalesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<IEnumerable<SaleDto>, Error>>;
