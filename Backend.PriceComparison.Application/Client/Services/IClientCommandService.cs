using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

namespace Backend.PriceComparison.Application.Client.Services;

public interface IClientCommandService
{
    Task<Result<VoidResult, Error>> CreateNaturalClientAsync(
        CreateClientNaturalPosCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<VoidResult, Error>> CreateLegalClientAsync(
        CreateClientLegalPosCommand command,
        CancellationToken cancellationToken = default);
}
