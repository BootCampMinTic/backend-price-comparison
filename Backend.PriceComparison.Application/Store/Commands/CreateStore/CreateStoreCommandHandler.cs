using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.CreateStore;

public sealed class CreateStoreCommandHandler(
    IStoreRepository _storeRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateStoreCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateStoreCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<StoreEntity>(request);
        var result = await _storeRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync(CacheKeys.StoresPrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
