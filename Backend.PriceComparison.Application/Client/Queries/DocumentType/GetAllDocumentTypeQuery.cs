using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Application.Client.Queries.DocumentType
{
    public record GetAllDocumentTypeQuery : IRequest<Result<ApiResponseDto<IEnumerable<DocumentTypeEntity>>, Error>>
    {
    }
}
