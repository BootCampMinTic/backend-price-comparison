using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;

public sealed class GetAllCategoryProductsQueryHandler(
    ICategoryProductRepository categoryProductRepository,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<GetAllCategoryProductsQuery, Result<IEnumerable<CategoryProductDto>, Error>>
{
    public async Task<Result<IEnumerable<CategoryProductDto>, Error>> Handle(
        GetAllCategoryProductsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.CategoryProductPage(request.PageNumber, request.PageSize);
        var cached = await cacheService.GetAsync<IEnumerable<CategoryProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await categoryProductRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = mapper.Map<IEnumerable<CategoryProductDto>>(result.Value!);
        await cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);
        return dtos.ToList();
    }
}
