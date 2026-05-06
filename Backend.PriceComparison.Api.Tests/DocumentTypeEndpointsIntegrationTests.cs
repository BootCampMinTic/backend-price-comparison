using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.PriceComparison.Application.Client.Dtos;

namespace Backend.PriceComparison.Api.Tests;

public class DocumentTypeEndpointsIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public DocumentTypeEndpointsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        WireMockStubs.SetupDefaultDocumentTypeStubs(factory.WireMock);
    }

    private HttpClient CreateClient()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
        return client;
    }

    [Fact]
    public async Task GetAllDocumentTypes_ReturnsOkWithData()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/v1/client/document-type");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<IEnumerable<DocumentTypeDto>>>();
        Assert.NotNull(result);
        Assert.Equal("success", result.Status);
        Assert.NotEmpty(result.Data!);

        var documentTypes = result.Data!.ToList();
        Assert.Equal(3, documentTypes.Count);
        Assert.All(documentTypes, dt => Assert.False(string.IsNullOrWhiteSpace(dt.DocumentType)));
    }

    [Fact]
    public async Task GetAllDocumentTypes_ContainsCCAndNIT()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/v1/client/document-type");

        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<IEnumerable<DocumentTypeDto>>>();
        Assert.NotNull(result);

        var types = result.Data!.Select(d => d.DocumentType).ToHashSet();

        Assert.Contains("CC", types);
        Assert.Contains("CE", types);
        Assert.Contains("NIT", types);
    }

    [Fact]
    public async Task GetAllDocumentTypes_SecondCallHitsCache()
    {
        var client = CreateClient();
        var first = await client.GetAsync("/api/v1/client/document-type");
        var second = await client.GetAsync("/api/v1/client/document-type");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var firstBody = await first.Content.ReadAsStringAsync();
        var secondBody = await second.Content.ReadAsStringAsync();

        var firstJson = JsonSerializer.Deserialize<JsonElement>(firstBody);
        var secondJson = JsonSerializer.Deserialize<JsonElement>(secondBody);

        Assert.Equal(
            firstJson.GetProperty("data").GetArrayLength(),
            secondJson.GetProperty("data").GetArrayLength());
    }
}
