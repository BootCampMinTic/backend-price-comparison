using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryStore;

public sealed class GetCategoryStoreByIdQueryHandler(
    ICategoryStoreRepository _categoryStoreRepository,
    ICacheService _cacheService)
    : IRequestHandler<GetCategoryStoreByIdQuery, Result<CategoryStoreDto, Error>>
{
    private static string CacheKey(int id) => $"categorystore:{id}";

    public async Task<Result<CategoryStoreDto, Error>> Handle(
        GetCategoryStoreByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKey(request.Id);
        var cached = await _cacheService.GetAsync<CategoryStoreDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _categoryStoreRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = new CategoryStoreDto { Id = result.Value!.Id, Description = result.Value!.Description };
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
