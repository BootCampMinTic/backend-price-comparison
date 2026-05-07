using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

public sealed class GetAllCategoryStoresQueryHandler(
    ICategoryStoreRepository _categoryStoreRepository,
    ICacheService _cacheService)
    : IRequestHandler<GetAllCategoryStoresQuery, Result<IEnumerable<CategoryStoreDto>, Error>>
{
    private const string CacheKey = CacheKeys.CategoryStoresAll;

    public async Task<Result<IEnumerable<CategoryStoreDto>, Error>> Handle(
        GetAllCategoryStoresQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync<IEnumerable<CategoryStoreDto>>(CacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _categoryStoreRepository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = result.Value!.Select(e => new CategoryStoreDto { Id = e.Id, Description = e.Description }).ToList();
        await _cacheService.SetAsync(CacheKey, dtos, expiration: null, cancellationToken);
        return dtos;
    }
}
