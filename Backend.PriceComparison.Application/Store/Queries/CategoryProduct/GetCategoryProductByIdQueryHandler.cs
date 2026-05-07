using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;

public sealed class GetCategoryProductByIdQueryHandler(
    ICategoryProductRepository _categoryProductRepository,
    ICacheService _cacheService)
    : IRequestHandler<GetCategoryProductByIdQuery, Result<CategoryProductDto, Error>>
{
    private static string CacheKey(int id) => $"categoryproduct:{id}";

    public async Task<Result<CategoryProductDto, Error>> Handle(
        GetCategoryProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKey(request.Id);
        var cached = await _cacheService.GetAsync<CategoryProductDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _categoryProductRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = new CategoryProductDto { Id = result.Value!.Id, Description = result.Value!.Description };
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
