using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.RegisterPrice;

/// <summary>
/// Manejador para procesar el comando de registro de precio de un producto en un supermercado.
/// </summary>
public sealed class RegisterPriceCommandHandler(
    IPriceHistoryRepository _priceHistoryRepository,
    IProductRepository _productRepository,
    IStoreRepository _storeRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<RegisterPriceCommand, Result<VoidResult, Error>>
{
    /// <summary>
    /// Procesa el comando, validando la existencia del producto y supermercado antes de guardar en la persistencia.
    /// </summary>
    public async Task<Result<VoidResult, Error>> Handle(
        RegisterPriceCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validar existencia del producto
        var productResult = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (!productResult.IsSuccess)
        {
            return productResult.Error!;
        }

        // 2. Validar existencia del supermercado (store)
        var storeResult = await _storeRepository.GetByIdAsync(request.StoreId, cancellationToken);
        if (!storeResult.IsSuccess)
        {
            return storeResult.Error!;
        }

        // 3. Mapear y registrar el historial de precio
        var entity = _mapper.Map<PriceHistoryEntity>(request);
        var result = await _priceHistoryRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error!;
        }

        // 4. Invalidar caché del comparador de precios de este producto específico
        await _cacheService.RemoveByPrefixAsync($"price_history_{request.ProductId}", cancellationToken);

        return VoidResult.Instance;
    }
}
