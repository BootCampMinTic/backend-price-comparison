using AutoMapper;
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

public class GetClientByDocumentNumberQueryHandler(
IClientDomainService _serverDomainService,
      IMapper _mapper,
      ICacheService _cacheService)
      : IRequestHandler<GetClientByDocumentNumberQuery, Result<ClientDto, Error>>
{
    public async Task<Result<ClientDto, Error>> Handle(
        GetClientByDocumentNumberQuery request,
 CancellationToken cancellationToken)
    {
        var cacheKey = $"client:{request.Type}:document:{request.DocumentNumber}";

        var cachedClient = await _cacheService.GetAsync<ClientDto>(cacheKey, cancellationToken);
        if (cachedClient != null)
            return cachedClient;

        var result = await _serverDomainService.GetByDocumentNumberAsync(request.DocumentNumber, request.Type, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var clientDto = _mapper.Map<ClientDto>(result.Value);

        await _cacheService.SetAsync(cacheKey, clientDto, null, cancellationToken);

        return clientDto;
    }
}
