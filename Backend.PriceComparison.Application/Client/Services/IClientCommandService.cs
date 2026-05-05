using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

namespace Backend.PriceComparison.Application.Client.Services;

/// <summary>
/// Application-level facade for client write operations. Each method
/// dispatches a MediatR command and returns a <see cref="Result{TValue, TError}"/>
/// so callers do not need to depend on <c>IMediator</c> directly.
/// </summary>
public interface IClientCommandService
{
    /// <summary>Creates a natural person client.</summary>
    /// <param name="command">Command carrying the natural client payload.</param>
    /// <param name="cancellationToken">Token to cancel the dispatch.</param>
    /// <returns>A successful <see cref="VoidResult"/> or an <see cref="Error"/> describing the failure.</returns>
    Task<Result<VoidResult, Error>> CreateNaturalClientAsync(
        CreateClientNaturalPosCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Creates a legal person client.</summary>
    /// <param name="command">Command carrying the legal client payload.</param>
    /// <param name="cancellationToken">Token to cancel the dispatch.</param>
    /// <returns>A successful <see cref="VoidResult"/> or an <see cref="Error"/> describing the failure.</returns>
    Task<Result<VoidResult, Error>> CreateLegalClientAsync(
        CreateClientLegalPosCommand command,
        CancellationToken cancellationToken = default);
}
