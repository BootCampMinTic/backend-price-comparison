using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Queries.Sale;

public record GetAllSalesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<IEnumerable<SaleDto>, Error>>;
