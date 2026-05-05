using AutoMapper;
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Common.Interfaces;
using Backend.PriceComparison.Domain.ClientPos.DomainServices;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

public class CreateClientNaturalPosHandle(
    IClientDomainService _serverDomainService,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateClientNaturalPosCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateClientNaturalPosCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ClientNaturalPosEntity>(request);
        var result = await _serverDomainService.CreateClientNaturalAsync(entity, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync("clients:natural", cancellationToken);

        return result.Value!;
    }
}
