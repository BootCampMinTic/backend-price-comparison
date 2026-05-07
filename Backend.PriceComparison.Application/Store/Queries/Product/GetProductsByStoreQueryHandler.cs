using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

public sealed class GetProductsByStoreQueryHandler(
    IProductRepository _productRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetProductsByStoreQuery, Result<IEnumerable<ProductDto>, Error>>
{
    public async Task<Result<IEnumerable<ProductDto>, Error>> Handle(
        GetProductsByStoreQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductsByStore(request.StoreId, request.PageNumber, request.PageSize);
        var cached = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _productRepository.GetByStoreAsync(request.StoreId, request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = _mapper.Map<IEnumerable<ProductDto>>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);
        return dtos.ToList();
    }
}
