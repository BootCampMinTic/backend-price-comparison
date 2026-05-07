using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;

public sealed class CreateCategoryProductCommandHandler(
    ICategoryProductRepository _categoryProductRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateCategoryProductCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateCategoryProductCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<CategoryProductEntity>(request);
        var result = await _categoryProductRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveAsync(CacheKeys.CategoryProductsAll, cancellationToken);
        return VoidResult.Instance;
    }
}
