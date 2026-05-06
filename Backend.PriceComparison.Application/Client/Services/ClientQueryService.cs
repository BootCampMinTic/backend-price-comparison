using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Application.Client.Queries.Client;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Application.Client.Services;

public sealed class ClientQueryService(IMediator mediator) : IClientQueryService
{
    public async Task<Result<IEnumerable<ClientDto>, Error>> GetAllLegalClientsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllClientLegalQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<IEnumerable<ClientDto>, Error>> GetAllNaturalClientsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllClientNaturalQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<ClientDto, Error>> GetClientByIdAsync(
        int id,
        string clientType,
        CancellationToken cancellationToken = default)
    {
        var type = clientType.Equals("natural", StringComparison.CurrentCultureIgnoreCase) ? ClientType.Natural : ClientType.Legal;
        var query = new GetClientByIdQuery
        {
            Id = id,
            Type = type
        };
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<Result<ClientDto, Error>> GetClientByDocumentNumberAsync(
        string documentNumber,
        string clientType,
        CancellationToken cancellationToken = default)
    {
        var type = clientType.Equals("natural", StringComparison.CurrentCultureIgnoreCase) ? ClientType.Natural : ClientType.Legal;
        var query = new GetClientByDocumentNumberQuery
        {
            DocumentNumber = documentNumber,
            Type = type
        };
        return await mediator.Send(query, cancellationToken);
    }
}
