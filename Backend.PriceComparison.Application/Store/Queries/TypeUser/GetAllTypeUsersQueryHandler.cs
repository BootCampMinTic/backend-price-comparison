using MediatR;
using Backend.PriceComparison.Application.Common;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.TypeUser;

public sealed class GetAllTypeUsersQueryHandler(
    ITypeUserRepository _typeUserRepository,
    ICacheService _cacheService)
    : IRequestHandler<GetAllTypeUsersQuery, Result<IEnumerable<TypeUserDto>, Error>>
{
    private const string CacheKey = CacheKeys.TypeUsersAll;

    public async Task<Result<IEnumerable<TypeUserDto>, Error>> Handle(
        GetAllTypeUsersQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync<IEnumerable<TypeUserDto>>(CacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _typeUserRepository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = result.Value!.Select(e => new TypeUserDto { Id = e.Id, Description = e.Description }).ToList();
        await _cacheService.SetAsync(CacheKey, dtos, expiration: null, cancellationToken);
        return dtos;
    }
}
