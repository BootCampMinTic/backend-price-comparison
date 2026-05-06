using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;

namespace Backend.PriceComparison.Application.Client.Queries.DocumentType
{
    /// <summary>
    /// Query to retrieve all document types from the catalog (CC, CE, NIT, etc.).
    /// </summary>
    public record GetAllDocumentTypeQuery : IRequest<Result<ApiResponseDto<IEnumerable<DocumentTypeDto>>, Error>>
    {
    }
}
