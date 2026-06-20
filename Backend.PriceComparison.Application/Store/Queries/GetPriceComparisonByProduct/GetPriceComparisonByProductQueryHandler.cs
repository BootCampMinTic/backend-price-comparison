using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.GetPriceComparisonByProduct;

/// <summary>
/// Manejador para la consulta que obtiene la comparación de precios de un producto en diferentes supermercados.
/// </summary>
public sealed class GetPriceComparisonByProductQueryHandler(
    IPriceHistoryRepository _priceHistoryRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetPriceComparisonByProductQuery, Result<IEnumerable<PriceHistoryDto>, Error>>
{
    /// <summary>
    /// Procesa la consulta utilizando caché distribuida (o memoria) para mejorar el rendimiento de la comparación de precios.
    /// </summary>
    public async Task<Result<IEnumerable<PriceHistoryDto>, Error>> Handle(
        GetPriceComparisonByProductQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"price_history_{request.ProductId}";

        // 1. Intentar obtener el resultado desde la caché
        var cachedData = await _cacheService.GetAsync<IEnumerable<PriceHistoryDto>>(cacheKey, cancellationToken);
        if (cachedData is not null)
        {
            return Result<IEnumerable<PriceHistoryDto>, Error>.Success(cachedData);
        }

        // 2. Si no está en caché, consultar el repositorio
        var repositoryResult = await _priceHistoryRepository.GetPricesByProductAsync(request.ProductId, cancellationToken);
        if (!repositoryResult.IsSuccess)
        {
            return repositoryResult.Error!;
        }

        // 3. Mapear entidades a DTOs
        var dtos = _mapper.Map<IEnumerable<PriceHistoryDto>>(repositoryResult.Value);

        // 4. Guardar en la caché por 10 minutos
        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);

        return Result<IEnumerable<PriceHistoryDto>, Error>.Success(dtos);
    }
}
