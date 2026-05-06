using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Application.Client.Queries.Client
{
    /// <summary>
    /// Query to retrieve a single client by its identifier and type.
    /// </summary>
    public record GetClientByIdQuery : IRequest<Result<ClientDto, Error>>
    {
        /// <summary>Client unique identifier.</summary>
        public int Id { get; init; }
        /// <summary>Client category: Natural or Legal.</summary>
        public ClientType Type { get; init; }
    }
}
