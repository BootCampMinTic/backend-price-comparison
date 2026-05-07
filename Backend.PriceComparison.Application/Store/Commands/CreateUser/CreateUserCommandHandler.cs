using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Store.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository _userRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateUserCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UserEntity>(request);
        var result = await _userRepository.CreateAsync(entity, cancellationToken);

        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync(CacheKeys.UsersPrefix, cancellationToken);
        return VoidResult.Instance;
    }
}
