using System.Net.Http.Json;
using System.Text.Json;
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;
using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Ports;

namespace Backend.PriceComparison.Api.Tests;

public class WireMockClientRepository : IClientRepository
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public WireMockClientRepository(HttpClient http, string baseUrl)
    {
        _http = http;
        _baseUrl = baseUrl;
    }

    public async Task<Result<VoidResult, Error>> CreateClientLegalAsync(ClientLegalPosEntity request, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync($"{_baseUrl}/clients/legal", request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(ct);
            var error = JsonSerializer.Deserialize<Error>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return error!;
        }
        var body = await response.Content.ReadAsStringAsync(ct);
        var entity = JsonSerializer.Deserialize<ClientLegalPosEntity>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return VoidResult.Instance;
    }

    public async Task<Result<VoidResult, Error>> CreateClientNaturalAsync(ClientNaturalPosEntity request, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync($"{_baseUrl}/clients/natural", request, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(ct);
            var error = JsonSerializer.Deserialize<Error>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return error!;
        }
        return VoidResult.Instance;
    }

    public async Task<Result<IEnumerable<ClientLegalPosEntity>, Error>> GetAllLegalAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var response = await _http.GetAsync($"{_baseUrl}/clients/legal?page={pageNumber}&size={pageSize}", ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(ct);
            var error = JsonSerializer.Deserialize<Error>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return error!;
        }
        var entities = await response.Content.ReadFromJsonAsync<List<ClientLegalPosEntity>>(ct);
        return entities!;
    }

    public async Task<Result<IEnumerable<ClientNaturalPosEntity>, Error>> GetAllNaturalAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var response = await _http.GetAsync($"{_baseUrl}/clients/natural?page={pageNumber}&size={pageSize}", ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(ct);
            var error = JsonSerializer.Deserialize<Error>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return error!;
        }
        var entities = await response.Content.ReadFromJsonAsync<List<ClientNaturalPosEntity>>(ct);
        return entities!;
    }

    public async Task<Result<ClientEntity, Error>> GetByIdAsync(int id, ClientType type, CancellationToken ct)
    {
        var clientType = type == ClientType.Natural ? "natural" : "legal";
        var response = await _http.GetAsync($"{_baseUrl}/clients/{clientType}/{id}", ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(ct);
            var error = JsonSerializer.Deserialize<Error>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return error!;
        }
        var body = await response.Content.ReadAsStringAsync(ct);
        ClientEntity entity = type == ClientType.Natural
            ? JsonSerializer.Deserialize<ClientNaturalPosEntity>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
            : JsonSerializer.Deserialize<ClientLegalPosEntity>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        return entity;
    }

    public async Task<Result<ClientEntity, Error>> GetByDocumentNumberAsync(string documentNumber, ClientType type, CancellationToken ct)
    {
        var clientType = type == ClientType.Natural ? "natural" : "legal";
        var response = await _http.GetAsync($"{_baseUrl}/clients/{clientType}/document/{documentNumber}", ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync(ct);
            var error = JsonSerializer.Deserialize<Error>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return error!;
        }
        var body = await response.Content.ReadAsStringAsync(ct);
        ClientEntity entity = type == ClientType.Natural
            ? JsonSerializer.Deserialize<ClientNaturalPosEntity>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
            : JsonSerializer.Deserialize<ClientLegalPosEntity>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        return entity;
    }
}
