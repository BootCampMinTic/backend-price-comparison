using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Application.Client.Queries.Client
{
    public class GetClientByDocumentNumberQuery : IRequest<Result<ClientDto, Error>>
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public ClientType Type { get; set; }
    }

}
