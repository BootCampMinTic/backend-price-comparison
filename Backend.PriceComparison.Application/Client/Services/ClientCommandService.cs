using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;

namespace Backend.PriceComparison.Application.Client.Services;

public class ClientCommandService : IClientCommandService
{
    private readonly IMediator _mediator;

    public ClientCommandService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Result<VoidResult, Error>> CreateNaturalClientAsync(
        CreateClientNaturalPosCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result<VoidResult, Error>> CreateLegalClientAsync(
        CreateClientLegalPosCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
    }
}
