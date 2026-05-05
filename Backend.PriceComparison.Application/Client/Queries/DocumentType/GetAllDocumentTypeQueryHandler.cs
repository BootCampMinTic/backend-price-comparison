using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.DocumentType;

public class GetAllDocumentTypeQueryHandler(
 IDocumentTypeDomainService _serverDomainService,
ICacheService _cacheService)
    : IRequestHandler<GetAllDocumentTypeQuery, Result<ApiResponseDto<IEnumerable<DocumentTypeDto>>, Error>>
{
    public async Task<Result<ApiResponseDto<IEnumerable<DocumentTypeDto>>, Error>> Handle(
    GetAllDocumentTypeQuery request,
    CancellationToken cancellationToken)
    {
        var cacheKey = "documenttypes:all";

        var cachedDocumentTypes = await _cacheService.GetAsync<IEnumerable<DocumentTypeDto>>(cacheKey, cancellationToken);
        if (cachedDocumentTypes != null)
            return new ApiResponseDto<IEnumerable<DocumentTypeDto>>(cachedDocumentTypes.ToList());

        var result = await _serverDomainService.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var documentTypes = result.Value!.Select(documentType => new DocumentTypeDto
        {
            Id = documentType.Id,
            Name = documentType.Name,
            DocumentType = documentType.DocumentType,
            HelpTextHeader = documentType.HelpTextHeader,
            HelpText = documentType.HelpText,
            Regex = documentType.Regex,
            Fields = documentType.Fields
        }).ToList();

        await _cacheService.SetAsync(cacheKey, documentTypes, null, cancellationToken);

        return new ApiResponseDto<IEnumerable<DocumentTypeDto>>(documentTypes);
    }
}
