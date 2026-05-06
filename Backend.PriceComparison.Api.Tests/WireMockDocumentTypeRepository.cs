using System.Net.Http.Json;
using System.Text.Json;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Api.Tests;

public class WireMockDocumentTypeRepository : IDocumentTypeRepository
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public WireMockDocumentTypeRepository(HttpClient http, string baseUrl)
    {
        _http = http;
        _baseUrl = baseUrl;
    }

    public async Task<Result<IEnumerable<DocumentTypeEntity>, Error>> GetAllAsync(CancellationToken ct)
    {
        var response = await _http.GetAsync($"{_baseUrl}/document-types", ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(ct);
            var error = JsonSerializer.Deserialize<Error>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return error!;
        }
        var entities = await response.Content.ReadFromJsonAsync<List<DocumentTypeEntity>>(ct);
        return entities!;
    }
}
