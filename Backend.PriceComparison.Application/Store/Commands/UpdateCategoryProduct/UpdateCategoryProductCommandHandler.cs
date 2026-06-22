using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.UpdateCategoryProduct;

public sealed class UpdateCategoryProductCommandHandler(
    ICategoryProductRepository categoryProductRepository,
    ICacheService cacheService)
    : IRequestHandler<UpdateCategoryProductCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        UpdateCategoryProductCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new CategoryProductEntity { Id = request.Id, Description = request.Description };
        var result = await categoryProductRepository.UpdateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await cacheService.RemoveByPrefixAsync(CacheKeys.CategoryProductPrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
