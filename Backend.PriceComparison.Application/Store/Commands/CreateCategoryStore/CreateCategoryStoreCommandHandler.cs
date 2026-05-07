using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;

public sealed class CreateCategoryStoreCommandHandler(
    ICategoryStoreRepository _categoryStoreRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateCategoryStoreCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateCategoryStoreCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<CategoryStoreEntity>(request);
        var result = await _categoryStoreRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveAsync(CacheKeys.CategoryStoresAll, cancellationToken);
        return VoidResult.Instance;
    }
}
