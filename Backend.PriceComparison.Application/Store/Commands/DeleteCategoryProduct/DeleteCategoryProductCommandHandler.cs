using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.DeleteCategoryProduct;

public sealed class DeleteCategoryProductCommandHandler(
    ICategoryProductRepository categoryProductRepository,
    ICacheService cacheService)
    : IRequestHandler<DeleteCategoryProductCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        DeleteCategoryProductCommand request,
        CancellationToken cancellationToken)
    {
        var result = await categoryProductRepository.DeleteAsync(request.Id, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await cacheService.RemoveByPrefixAsync(CacheKeys.CategoryProductPrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
