using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

public sealed class GetCategoryStoreByIdQueryHandler(
    ICategoryStoreRepository categoryStoreRepository,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<GetCategoryStoreByIdQuery, Result<CategoryStoreDto, Error>>
{
    public async Task<Result<CategoryStoreDto, Error>> Handle(
        GetCategoryStoreByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.CategoryStoreById(request.Id);
        var cached = await cacheService.GetAsync<CategoryStoreDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await categoryStoreRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = mapper.Map<CategoryStoreDto>(result.Value!);
        await cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
