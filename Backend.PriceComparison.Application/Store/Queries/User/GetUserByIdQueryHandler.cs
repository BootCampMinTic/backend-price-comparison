using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Queries.User;

public sealed class GetUserByIdQueryHandler(
    IUserRepository _userRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto, Error>>
{
    public async Task<Result<UserDto, Error>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.UserById(request.Id);
        var cached = await _cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = _mapper.Map<UserDto>(result.Value!);
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
