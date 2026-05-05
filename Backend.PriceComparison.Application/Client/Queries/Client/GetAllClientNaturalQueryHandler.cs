using AutoMapper;
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

public class GetAllClientNaturalQueryHandler(
    IClientRepository _clientRepository,
    ICacheService _cacheService,
    IMapper _mapper)
    : IRequestHandler<GetAllClientNaturalQuery, Result<IEnumerable<ClientDto>, Error>>
{
    public async Task<Result<IEnumerable<ClientDto>, Error>> Handle(
        GetAllClientNaturalQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"clients:natural:page:{request.PageNumber}:size:{request.PageSize}";

        var cached = await _cacheService.GetAsync<IEnumerable<ClientDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached.ToList();

        var result = await _clientRepository.GetAllNaturalAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = _mapper.Map<IEnumerable<ClientDto>>(result.Value).ToList();
        await _cacheService.SetAsync(cacheKey, dtos, expiration: null, cancellationToken);
        return dtos;
    }
}
