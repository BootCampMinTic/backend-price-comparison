using MediatR;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;
public record GetCategoryProductByIdQuery(int Id) : IRequest<Result<CategoryProductDto, Error>>;
GetCategoryProductByIdQueryHandler.cs
using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
namespace Backend.PriceComparison.Application.Store.Queries.CategoryProduct;
public sealed class GetCategoryProductByIdQueryHandler(
    ICategoryProductRepository _categoryProductRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetCategoryProductByIdQuery, Result<CategoryProductDto, Error>>
{
    public async Task<Result<CategoryProductDto, Error>> Handle(
        GetCategoryProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.CategoryProductById(request.Id);
        var cached = await _cacheService.GetAsync<CategoryProductDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;
        var result = await _categoryProductRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;
        var dto = _mapper.Map<CategoryProductDto>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
