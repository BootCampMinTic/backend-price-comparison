using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;

public sealed class CreateCategoryStoreCommandHandler(
    ICategoryStoreRepository categoryStoreRepository,
    ICacheService cacheService)
    : IRequestHandler<CreateCategoryStoreCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateCategoryStoreCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new CategoryStoreEntity { Description = request.Description };
        var result = await categoryStoreRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await cacheService.RemoveByPrefixAsync(CacheKeys.CategoryStorePrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
