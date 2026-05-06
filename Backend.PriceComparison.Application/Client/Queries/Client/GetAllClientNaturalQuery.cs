using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;

namespace Backend.PriceComparison.Application.Client.Queries.Client;

/// <summary>
/// Query to retrieve a paginated list of all natural person clients.
/// </summary>
public record GetAllClientNaturalQuery : IRequest<Result<IEnumerable<ClientDto>, Error>>
{
    /// <summary>Page number (1-based).</summary>
    public int PageNumber { get; set; }
    /// <summary>Number of records per page.</summary>
    public int PageSize { get; set; }
}
