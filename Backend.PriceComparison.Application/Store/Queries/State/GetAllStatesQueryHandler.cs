using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.State;

public sealed class GetAllStatesQueryHandler(
    IStateRepository _stateRepository,
    ICacheService _cacheService)
    : IRequestHandler<GetAllStatesQuery, Result<IEnumerable<StateDto>, Error>>
{
    private const string CacheKey = CacheKeys.StatesAll;

    public async Task<Result<IEnumerable<StateDto>, Error>> Handle(
        GetAllStatesQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync<IEnumerable<StateDto>>(CacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _stateRepository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = result.Value!.Select(e => new StateDto { Id = e.Id, Description = e.Description }).ToList();
        await _cacheService.SetAsync(CacheKey, dtos, expiration: null, cancellationToken);
        return dtos;
    }
}
