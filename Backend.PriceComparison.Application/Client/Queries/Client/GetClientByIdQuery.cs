using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Application.Client.Queries.Client
{
    public record GetClientByIdQuery : IRequest<Result<ClientDto, Error>>
    {
        public int Id { get; init; }
        public ClientType Type { get; init; }
    }
}
