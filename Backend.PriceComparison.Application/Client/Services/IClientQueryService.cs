using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;

namespace Backend.PriceComparison.Application.Client.Services;

public interface IClientQueryService
{
    Task<Result<IEnumerable<ClientDto>, Error>> GetAllLegalClientsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<ClientDto>, Error>> GetAllNaturalClientsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<ClientDto, Error>> GetClientByIdAsync(
        int id,
        string clientType,
        CancellationToken cancellationToken = default);

    Task<Result<ClientDto, Error>> GetClientByDocumentNumberAsync(
        string documentNumber,
        string clientType,
        CancellationToken cancellationToken = default);
}
