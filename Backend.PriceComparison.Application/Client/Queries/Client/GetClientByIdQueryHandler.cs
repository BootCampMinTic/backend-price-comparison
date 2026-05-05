using AutoMapper;
using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

public class GetClientByIdQueryHandler(
    IClientRepository _clientRepository,
    IMapper _mapper,
    ICacheService _cacheService)
    : IRequestHandler<GetClientByIdQuery, Result<ClientDto, Error>>
{
    public async Task<Result<ClientDto, Error>> Handle(
        GetClientByIdQuery request,
        CancellationToken cancellationToken)
    {
        // MOCK: Respuesta simulada para cliente por ID
        var mockClient = request.Type.ToString() == "Natural"
            ? new ClientDto { Id = request.Id, Name = "Juan", LastName = "Perez", DocumentNumber = "12345678", DocumentTypeId = 1 }
            : new ClientDto { Id = request.Id, CompanyName = "Empresa ABC", DocumentNumber = "9001234567", DocumentTypeId = 3, VerificationDigit = 1 };

        // Ejemplo de cómo sería con persistencia (comentado):
        /*
        var cacheKey = $"client:{request.Type}:{request.Id}";
        var cachedClient = await _cacheService.GetAsync<ClientDto>(cacheKey, cancellationToken);
        if (cachedClient != null)
            return cachedClient;

        var result = await _clientRepository.GetByIdAsync(request.Id, request.Type, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var clientDto = _mapper.Map<ClientDto>(result.Value);
        await _cacheService.SetAsync(cacheKey, clientDto, null, cancellationToken);
        */

        return mockClient;
    }
}
