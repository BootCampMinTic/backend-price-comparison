using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;
public sealed class GetAllCategoryProductsQueryHandler(
    ICategoryProductRepository _categoryProductRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetAllCategoryProductsQuery, Result<IEnumerable<CategoryProductDto>, Error>>
{
    public async Task<Result<IEnumerable<CategoryProductDto>, Error>> Handle(
        GetAllCategoryProductsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.CategoryProductsPrefix;
        var cached = await _cacheService.GetAsync<IEnumerable<CategoryProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();
        var result = await _categoryProductRepository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;
        var dtos = _mapper.Map<IEnumerable<CategoryProductDto>>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);
        return dtos.ToList();
    }
}
