using AutoMapper;
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

public class CreateClientLegalPosHandle(
    IClientDomainService _serverDomainService,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<CreateClientLegalPosCommand, Result<VoidResult, Error>>
{
    public async Task<Result<VoidResult, Error>> Handle(
        CreateClientLegalPosCommand request,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ClientLegalPosEntity>(request);
        var result = await _serverDomainService.CreateClientLegalAsync(entity, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        await _cacheService.RemoveByPrefixAsync("clients:legal", cancellationToken);

        return result.Value!;
    }
}
