using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Application.Client.Queries.Client
{
    /// <summary>
    /// Query to retrieve a single client by its document number and type.
    /// </summary>
    public class GetClientByDocumentNumberQuery : IRequest<Result<ClientDto, Error>>
    {
        /// <summary>National identification or tax document number.</summary>
        public string DocumentNumber { get; set; } = string.Empty;
        /// <summary>Client category: Natural or Legal.</summary>
        public ClientType Type { get; set; }
    }

}
