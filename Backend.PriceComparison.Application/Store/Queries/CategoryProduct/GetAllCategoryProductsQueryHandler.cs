using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;

public sealed class GetAllCategoryProductsQueryHandler(
    ICategoryProductRepository _categoryProductRepository,
    ICacheService _cacheService)
    : IRequestHandler<GetAllCategoryProductsQuery, Result<IEnumerable<CategoryProductDto>, Error>>
{
    private const string CacheKey = CacheKeys.CategoryProductsAll;

    public async Task<Result<IEnumerable<CategoryProductDto>, Error>> Handle(
        GetAllCategoryProductsQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync<IEnumerable<CategoryProductDto>>(CacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _categoryProductRepository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = result.Value!.Select(e => new CategoryProductDto { Id = e.Id, Description = e.Description }).ToList();
        await _cacheService.SetAsync(CacheKey, dtos, expiration: null, cancellationToken);
        return dtos;
    }
}
