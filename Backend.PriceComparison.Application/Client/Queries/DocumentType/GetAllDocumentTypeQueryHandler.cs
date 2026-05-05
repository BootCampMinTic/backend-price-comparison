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
    public async Task<Result<ApiResponseDto<IEnumerable<DocumentTypeDto>>, Error>> Handle(
    GetAllDocumentTypeQuery request,
    CancellationToken cancellationToken)
    {
        // MOCK: Respuesta simulada para document types
        var mockDocumentTypes = new List<DocumentTypeDto>
        {
            new() { Id = 1, Name = "Cedula de ciudadania", DocumentType = "CC", HelpTextHeader = "Ingrese su cedula", HelpText = "Numero de identificacion", Regex = "^[0-9]{6,10}$", Fields = "numero" },
            new() { Id = 2, Name = "Cedula de extranjeria", DocumentType = "CE", HelpTextHeader = "Ingrese su cedula de extranjeria", HelpText = "Numero de identificacion extranjera", Regex = "^[0-9]{6,10}$", Fields = "numero" },
            new() { Id = 3, Name = "NIT", DocumentType = "NIT", HelpTextHeader = "Ingrese el NIT", HelpText = "Numero de identificacion tributaria", Regex = "^[0-9]{9,10}$", Fields = "numero,dv" }
        };

        // Ejemplo de cómo sería con persistencia (comentado):
        /*
        var cacheKey = "documenttypes:all";
        var cachedDocumentTypes = await _cacheService.GetAsync<IEnumerable<DocumentTypeDto>>(cacheKey, cancellationToken);
        if (cachedDocumentTypes != null)
            return new ApiResponseDto<IEnumerable<DocumentTypeDto>>(cachedDocumentTypes.ToList());

        var result = await _documentTypeRepository.GetAllAsync(cancellationToken);
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
        */

        return new ApiResponseDto<IEnumerable<DocumentTypeDto>>(mockDocumentTypes);
    }
}
