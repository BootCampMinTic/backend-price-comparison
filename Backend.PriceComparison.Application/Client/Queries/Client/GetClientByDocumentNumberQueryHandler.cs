using AutoMapper;
using MediatR;
using Backend.PriceComparison.Application.Client;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

public sealed class GetClientByDocumentNumberQueryHandler(
    IClientRepository _clientRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetClientByDocumentNumberQuery, Result<ClientDto, Error>>
{
    public async Task<Result<ClientDto, Error>> Handle(
        GetClientByDocumentNumberQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ClientByDocumentNumber(request.DocumentNumber, request.Type.ToString());

        var cached = await _cacheService.GetAsync<ClientDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _clientRepository.GetByDocumentNumberAsync(request.DocumentNumber, request.Type, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dto = _mapper.Map<ClientDto>(result.Value);
        await _cacheService.SetAsync(cacheKey, dto, expiration: null, cancellationToken);
        return dto;
    }
}
