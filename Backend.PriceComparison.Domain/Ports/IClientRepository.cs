using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Domain.Ports
{
    /// <summary>
    /// Persistence port for client aggregates (legal and natural persons).
    /// Implementations must be transactional and return a <see cref="Result{TValue, TError}"/>
    /// instead of throwing for expected business errors.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>Persists a new legal person client.</summary>
        /// <param name="request">Legal client entity to create.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A successful <see cref="VoidResult"/> or an <see cref="Error"/> describing the failure.</returns>
        Task<Result<VoidResult, Error>> CreateClientLegalAsync(ClientLegalPosEntity request, CancellationToken cancellationToken);

        /// <summary>Persists a new natural person client.</summary>
        /// <param name="request">Natural client entity to create.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A successful <see cref="VoidResult"/> or an <see cref="Error"/> describing the failure.</returns>
        Task<Result<VoidResult, Error>> CreateClientNaturalAsync(ClientNaturalPosEntity request, CancellationToken cancellationToken);

        /// <summary>Returns a paginated list of legal clients.</summary>
        /// <param name="pageNumber">1-based page index.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The page of legal clients or an <see cref="Error"/> when no records match.</returns>
        Task<Result<IEnumerable<ClientLegalPosEntity>, Error>> GetAllLegalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        /// <summary>Returns a paginated list of natural clients.</summary>
        /// <param name="pageNumber">1-based page index.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The page of natural clients or an <see cref="Error"/> when no records match.</returns>
        Task<Result<IEnumerable<ClientNaturalPosEntity>, Error>> GetAllNaturalAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        /// <summary>Looks up a client by its identifier within the requested category.</summary>
        /// <param name="id">Database identifier of the client.</param>
        /// <param name="type">Whether to query the legal or natural table.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The matching <see cref="ClientEntity"/> or a not-found <see cref="Error"/>.</returns>
        Task<Result<ClientEntity, Error>> GetByIdAsync(int id, ClientType type, CancellationToken cancellationToken);

        /// <summary>Looks up a client by its document number within the requested category.</summary>
        /// <param name="documentNumber">National document number to search by.</param>
        /// <param name="type">Whether to query the legal or natural table.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The matching <see cref="ClientEntity"/> or a not-found <see cref="Error"/>.</returns>
        Task<Result<ClientEntity, Error>> GetByDocumentNumberAsync(string documentNumber, ClientType type, CancellationToken cancellationToken);
    }
}
