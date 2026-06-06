using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

public sealed class GetAllCategoryStoresQueryHandler(
    ICategoryStoreRepository categoryStoreRepository,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<GetAllCategoryStoresQuery, Result<IEnumerable<CategoryStoreDto>, Error>>
{
    public async Task<Result<IEnumerable<CategoryStoreDto>, Error>> Handle(
        GetAllCategoryStoresQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.CategoryStorePage(request.PageNumber, request.PageSize);
        var cached = await cacheService.GetAsync<IEnumerable<CategoryStoreDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await categoryStoreRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = mapper.Map<IEnumerable<CategoryStoreDto>>(result.Value!);
        await cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);
        return dtos.ToList();
    }
}
