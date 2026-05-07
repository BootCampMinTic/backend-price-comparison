using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.User;

public sealed class GetAllUsersQueryHandler(
    IUserRepository _userRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserDto>, Error>>
{
    public async Task<Result<IEnumerable<UserDto>, Error>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync<IEnumerable<UserDto>>(CacheKeys.UsersAll, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _userRepository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = _mapper.Map<IEnumerable<UserDto>>(result.Value!);
        await _cacheService.SetAsync(CacheKeys.UsersAll, dtos, expiration: null, cancellationToken);
        return dtos.ToList();
    }
}
