using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.DomainServices;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Application.Common.Interfaces;

namespace Backend.PriceComparison.Application.Client.Queries.DocumentType;

public class GetAllDocumentTypeQueryHandler(
 IDocumentTypeDomainService _serverDomainService,
ICacheService _cacheService)
    : IRequestHandler<GetAllDocumentTypeQuery, Result<ApiResponseDto<IEnumerable<DocumentTypeEntity>>, Error>>
{
    public async Task<Result<ApiResponseDto<IEnumerable<DocumentTypeEntity>>, Error>> Handle(
    GetAllDocumentTypeQuery request,
    CancellationToken cancellationToken)
    {
        var cacheKey = "documenttypes:all";

        var cachedDocumentTypes = await _cacheService.GetAsync<IEnumerable<DocumentTypeEntity>>(cacheKey, cancellationToken);
        if (cachedDocumentTypes != null)
            return new ApiResponseDto<IEnumerable<DocumentTypeEntity>>(cachedDocumentTypes.ToList());

        var result = await _serverDomainService.GetAllAsync(cancellationToken);
        if (!result.IsSuccess && result.Value != null)
            return result.Error!;

        var documentTypes = result.Value.ToList();

        await _cacheService.SetAsync(cacheKey, documentTypes, null, cancellationToken);

        return new ApiResponseDto<IEnumerable<DocumentTypeEntity>>(documentTypes);
    }
}

