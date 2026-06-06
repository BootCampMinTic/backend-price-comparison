using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.DeleteCategoryStore;

public sealed class DeleteCategoryStoreCommandHandler(
    ICategoryStoreRepository categoryStoreRepository,
    ICacheService cacheService)
    : IRequestHandler<DeleteCategoryStoreCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        DeleteCategoryStoreCommand request,
        CancellationToken cancellationToken)
    {
        var result = await categoryStoreRepository.DeleteAsync(request.Id, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await cacheService.RemoveByPrefixAsync(CacheKeys.CategoryStorePrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
