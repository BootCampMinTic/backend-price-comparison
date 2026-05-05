using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;

namespace Backend.PriceComparison.Application.Client.Services;

/// <summary>
/// Application-level facade for client read operations. Each method
/// dispatches a MediatR query and maps the domain result to a <see cref="ClientDto"/>.
/// </summary>
public interface IClientQueryService
{
    /// <summary>Returns a paginated list of legal clients.</summary>
    /// <param name="pageNumber">1-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the dispatch.</param>
    /// <returns>The page of <see cref="ClientDto"/> or an <see cref="Error"/> when no records match.</returns>
    Task<Result<IEnumerable<ClientDto>, Error>> GetAllLegalClientsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a paginated list of natural clients.</summary>
    /// <param name="pageNumber">1-based page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the dispatch.</param>
    /// <returns>The page of <see cref="ClientDto"/> or an <see cref="Error"/> when no records match.</returns>
    Task<Result<IEnumerable<ClientDto>, Error>> GetAllNaturalClientsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Looks up a client by its identifier.</summary>
    /// <param name="id">Database identifier of the client.</param>
    /// <param name="clientType">Either <c>"natural"</c> or <c>"legal"</c> (case insensitive).</param>
    /// <param name="cancellationToken">Token to cancel the dispatch.</param>
    /// <returns>The matching <see cref="ClientDto"/> or a not-found <see cref="Error"/>.</returns>
    Task<Result<ClientDto, Error>> GetClientByIdAsync(
        int id,
        string clientType,
        CancellationToken cancellationToken = default);

    /// <summary>Looks up a client by its document number.</summary>
    /// <param name="documentNumber">National document number to search by.</param>
    /// <param name="clientType">Either <c>"natural"</c> or <c>"legal"</c> (case insensitive).</param>
    /// <param name="cancellationToken">Token to cancel the dispatch.</param>
    /// <returns>The matching <see cref="ClientDto"/> or a not-found <see cref="Error"/>.</returns>
    Task<Result<ClientDto, Error>> GetClientByDocumentNumberAsync(
        string documentNumber,
        string clientType,
        CancellationToken cancellationToken = default);
}
