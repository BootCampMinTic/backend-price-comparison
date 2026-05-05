using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

public record GetAllClientLegalQuery : IRequest<Result<IEnumerable<ClientDto>, Error>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
