using MediatR;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Application.Client.Queries.DocumentType;

public class GetAllDocumentTypeQueryHandler(
    IDocumentTypeRepository _documentTypeRepository,
    ICacheService _cacheService)
    : IRequestHandler<GetAllDocumentTypeQuery, Result<ApiResponseDto<IEnumerable<DocumentTypeDto>>, Error>>
{
    private const string CacheKey = "documenttypes:all";

    public async Task<Result<ApiResponseDto<IEnumerable<DocumentTypeDto>>, Error>> Handle(
        GetAllDocumentTypeQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync<IEnumerable<DocumentTypeDto>>(CacheKey, cancellationToken);
        if (cached is not null)
            return new ApiResponseDto<IEnumerable<DocumentTypeDto>>(cached.ToList());

        var result = await _documentTypeRepository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess)
            return result.Error!;

        var dtos = result.Value!.Select(entity => new DocumentTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            DocumentType = entity.DocumentType,
            HelpTextHeader = entity.HelpTextHeader,
            HelpText = entity.HelpText,
            Regex = entity.Regex,
            Fields = entity.Fields
        }).ToList();

        await _cacheService.SetAsync(CacheKey, dtos, expiration: null, cancellationToken);
        return new ApiResponseDto<IEnumerable<DocumentTypeDto>>(dtos);
    }
}
