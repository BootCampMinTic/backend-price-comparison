using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.UpdateCategoryStore;

public sealed class UpdateCategoryStoreCommandHandler(
    ICategoryStoreRepository categoryStoreRepository,
    ICacheService cacheService)
    : IRequestHandler<UpdateCategoryStoreCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        UpdateCategoryStoreCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new CategoryStoreEntity { Id = request.Id, Description = request.Description };
        var result = await categoryStoreRepository.UpdateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await cacheService.RemoveByPrefixAsync(CacheKeys.CategoryStorePrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
