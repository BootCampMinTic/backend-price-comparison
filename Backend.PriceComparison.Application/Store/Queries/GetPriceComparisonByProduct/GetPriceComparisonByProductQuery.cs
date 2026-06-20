using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;

namespace Backend.PriceComparison.Application.Store.Queries.GetPriceComparisonByProduct;

/// <summary>
/// Consulta CQRS para obtener la lista de precios de un producto comparado en diferentes supermercados.
/// </summary>
public record GetPriceComparisonByProductQuery(
    int ProductId
) : IRequest<Result<IEnumerable<PriceHistoryDto>, Error>>;
