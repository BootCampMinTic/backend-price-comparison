using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

namespace Backend.PriceComparison.Application.Client.Services;

public sealed class ClientCommandService(IMediator mediator) : IClientCommandService
{
    public async Task<Result<VoidResult, Error>> CreateNaturalClientAsync(
        CreateClientNaturalPosCommand command,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<Result<VoidResult, Error>> CreateLegalClientAsync(
        CreateClientLegalPosCommand command,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(command, cancellationToken);
    }
}
