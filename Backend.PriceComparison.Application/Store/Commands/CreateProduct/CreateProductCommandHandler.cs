using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository _productRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateProductCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ProductEntity>(request);
        var result = await _productRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync(CacheKeys.ProductsPrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
