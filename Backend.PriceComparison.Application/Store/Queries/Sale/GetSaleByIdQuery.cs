using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Queries.Sale;

public record GetSaleByIdQuery(int Id) : IRequest<Result<SaleDto, Error>>;
