using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.Product;

public sealed class GetProductByIdQueryHandler(
    IProductRepository _productRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto, Error>>
{
    public async Task<Result<ProductDto, Error>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductById(request.Id);
        var cached = await _cacheService.GetAsync<ProductDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = _mapper.Map<ProductDto>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
