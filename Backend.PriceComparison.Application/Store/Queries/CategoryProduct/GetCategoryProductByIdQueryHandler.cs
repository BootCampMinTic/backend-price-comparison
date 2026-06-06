using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;

public sealed class GetCategoryProductByIdQueryHandler(
    ICategoryProductRepository categoryProductRepository,
    IMapper mapper,
    ICacheService cacheService)
    : IRequestHandler<GetCategoryProductByIdQuery, Result<CategoryProductDto, Error>>
{
    public async Task<Result<CategoryProductDto, Error>> Handle(
        GetCategoryProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.CategoryProductById(request.Id);
        var cached = await cacheService.GetAsync<CategoryProductDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await categoryProductRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = mapper.Map<CategoryProductDto>(result.Value!);
        await cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
