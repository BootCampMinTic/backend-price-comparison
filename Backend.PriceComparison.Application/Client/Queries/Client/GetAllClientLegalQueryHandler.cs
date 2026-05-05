using AutoMapper;
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

public class GetAllClientLegalQueryHandler(
    IClientRepository _clientRepository,
    ICacheService _cacheService,
    IMapper _mapper)
    : IRequestHandler<GetAllClientLegalQuery, Result<IEnumerable<ClientDto>, Error>>
{
    public async Task<Result<IEnumerable<ClientDto>, Error>> Handle(
        GetAllClientLegalQuery request,
        CancellationToken cancellationToken)
    {
        // MOCK: Respuesta simulada para clientes legales
        var mockClients = new List<ClientDto>
        {
            new() { Id = 1, CompanyName = "Empresa ABC", DocumentNumber = "9001234567", DocumentTypeId = 3, VerificationDigit = 1 },
            new() { Id = 2, CompanyName = "Empresa XYZ", DocumentNumber = "9009876543", DocumentTypeId = 3, VerificationDigit = 7 }
        };

        // Ejemplo de cómo sería con persistencia (comentado):
        /*
        var cacheKey = $"clients:legal:page:{request.PageNumber}:size:{request.PageSize}";
        var cachedClients = await _cacheService.GetAsync<IEnumerable<ClientDto>>(cacheKey, cancellationToken);
        if (cachedClients != null)
            return cachedClients.ToList();

        var result = await _clientRepository.GetAllLegalAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (!result.IsSuccess && result.Value != null)
            return result.Error!;

        var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(result.Value).ToList();
        await _cacheService.SetAsync(cacheKey, clientDtos, null, cancellationToken);
        */

        return mockClients;
    }
}
