using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Store;

public sealed class GetStoreByIdQueryHandler(
    IStoreRepository _storeRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetStoreByIdQuery, Result<StoreDto, Error>>
{
    public async Task<Result<StoreDto, Error>> Handle(
        GetStoreByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.StoreById(request.Id);
        var cached = await _cacheService.GetAsync<StoreDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _storeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = _mapper.Map<StoreDto>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
