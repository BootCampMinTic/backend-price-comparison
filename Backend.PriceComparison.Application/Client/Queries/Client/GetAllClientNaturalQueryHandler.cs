using AutoMapper;
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

public class GetAllClientNaturalQueryHandler(
IClientDomainService _serverDomainService,
    ICacheService _cacheService,
 IMapper _mapper)
: IRequestHandler<GetAllClientNaturalQuery, Result<IEnumerable<ClientDto>, Error>>
{
    public async Task<Result<IEnumerable<ClientDto>, Error>> Handle(
  GetAllClientNaturalQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"clients:natural:page:{request.PageNumber}:size:{request.PageSize}";

        var cachedClients = await _cacheService.GetAsync<IEnumerable<ClientDto>>(cacheKey, cancellationToken);
        if (cachedClients != null)
            return cachedClients.ToList();

        var result = await _serverDomainService.GetAllNaturalAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess && result.Value != null)
            return result.Error!;

        var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(result.Value).ToList();

        await _cacheService.SetAsync(cacheKey, clientDtos, null, cancellationToken);

        return clientDtos;
    }
}
