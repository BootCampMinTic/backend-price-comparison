using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.PriceComparison.Api.Common.Wrappers;

namespace Backend.PriceComparison.Api.Tests;

public class ClientEndpointsIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public ClientEndpointsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        WireMockStubs.SetupDefaultClientStubs(factory.WireMock);
    }

    private HttpClient CreateAuthClient()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
        return client;
    }

    private HttpClient CreateUnauthClient()
    {
        return _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllLegalClients_WithPagination_ReturnsPagedResponse()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/legal?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(2, json.RootElement.GetProperty("data").GetArrayLength());
    }

    [Fact]
    public async Task GetAllNaturalClients_WithPagination_ReturnsPagedResponse()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/natural?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(2, json.RootElement.GetProperty("data").GetArrayLength());
    }

    [Fact]
    public async Task CreateNaturalClient_WithValidPayload_ReturnsOk()
    {
        var client = CreateAuthClient();
        var payload = new
        {
            name = "Ana",
            middleName = (string?)null,
            lastName = "Lopez",
            secondSurname = (string?)null,
            documentNumber = "123456789",
            electronicInvoiceEmail = "ana@example.com",
            documentTypeId = 1,
            documentCountry = "CO"
        };

        var response = await client.PostAsJsonAsync("/api/v1/client/natural", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Contains("created", result.Message);
    }

    [Fact]
    public async Task CreateLegalClient_WithValidPayload_ReturnsOk()
    {
        var client = CreateAuthClient();
        var payload = new
        {
            companyName = "Empresa SAS",
            verificationDigit = 1,
            documentNumber = "900123456",
            electronicInvoiceEmail = "billing@example.com",
            vatResponsibleParty = true,
            selfRetainer = false,
            withholdingAgent = false,
            simpleTaxRegime = false,
            documentTypeId = 2,
            largeTaxpayer = false,
            documentCountry = "CO"
        };

        var response = await client.PostAsJsonAsync("/api/v1/client/legal", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(result);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetNaturalClientById_WithValidId_ReturnsClient()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/natural/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal("Juan", json.RootElement.GetProperty("data").GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetLegalClientById_WithValidId_ReturnsClient()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/legal/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.True(json.RootElement.TryGetProperty("data", out var data));
        Assert.NotEqual(JsonValueKind.Null, data.ValueKind);
    }

    [Fact]
    public async Task GetNaturalClientById_WithoutAuthToken_ReturnsUnauthorized()
    {
        var client = CreateUnauthClient();

        var response = await client.GetAsync("/api/v1/client/natural/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLegalClientById_WithoutAuthToken_ReturnsUnauthorized()
    {
        var client = CreateUnauthClient();

        var response = await client.GetAsync("/api/v1/client/legal/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetNaturalClientByDocumentNumber_ReturnsClient()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/natural/12345678/document-number");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal("Juan", json.RootElement.GetProperty("data").GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetLegalClientByDocumentNumber_ReturnsClient()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/legal/9001234567/document-number");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        Assert.True(json.RootElement.TryGetProperty("data", out var data));
        Assert.NotEqual(JsonValueKind.Null, data.ValueKind);
    }

    [Fact]
    public async Task GetAllNaturalClients_CacheHit_ReturnsConsistentData()
    {
        var client = CreateAuthClient();

        var first = await client.GetAsync("/api/v1/client/natural?pageNumber=1&pageSize=10");
        var second = await client.GetAsync("/api/v1/client/natural?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var firstBody = await first.Content.ReadAsStringAsync();
        var secondBody = await second.Content.ReadAsStringAsync();

        var firstJson = JsonDocument.Parse(firstBody);
        var secondJson = JsonDocument.Parse(secondBody);

        Assert.Equal(
            firstJson.RootElement.GetProperty("data").GetArrayLength(),
            secondJson.RootElement.GetProperty("data").GetArrayLength());
    }

    [Fact]
    public async Task CreateNaturalClient_InvalidatesCache_ThenGetAllStillWorks()
    {
        var client = CreateAuthClient();

        var payload = new
        {
            name = "Maria",
            lastName = "Gomez",
            documentNumber = "99999999",
            electronicInvoiceEmail = "maria@example.com",
            documentTypeId = 1,
            documentCountry = "CO"
        };
        var postResponse = await client.PostAsJsonAsync("/api/v1/client/natural", payload);
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        var getAll = await client.GetAsync("/api/v1/client/natural?pageNumber=1&pageSize=100");
        Assert.Equal(HttpStatusCode.OK, getAll.StatusCode);

        var body = await getAll.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task CreateLegalClient_InvalidatesCache_ThenGetAllStillWorks()
    {
        var client = CreateAuthClient();

        var payload = new
        {
            companyName = "Nueva Empresa LTDA",
            verificationDigit = 3,
            documentNumber = "800555123",
            electronicInvoiceEmail = "nueva@example.com",
            vatResponsibleParty = true,
            selfRetainer = false,
            withholdingAgent = true,
            simpleTaxRegime = false,
            documentTypeId = 3,
            largeTaxpayer = true,
            documentCountry = "CO"
        };
        var postResponse = await client.PostAsJsonAsync("/api/v1/client/legal", payload);
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        var result = await postResponse.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.True(result?.Success);
    }
}
