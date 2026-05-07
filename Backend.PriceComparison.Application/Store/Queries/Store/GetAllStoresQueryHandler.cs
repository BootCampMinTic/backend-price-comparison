using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Store;

public sealed class GetAllStoresQueryHandler(
    IStoreRepository _storeRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetAllStoresQuery, Result<IEnumerable<StoreDto>, Error>>
{
    public async Task<Result<IEnumerable<StoreDto>, Error>> Handle(
        GetAllStoresQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.StoresPage(request.PageNumber, request.PageSize);
        var cached = await _cacheService.GetAsync<IEnumerable<StoreDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _storeRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = _mapper.Map<IEnumerable<StoreDto>>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);
        return dtos.ToList();
    }
}
