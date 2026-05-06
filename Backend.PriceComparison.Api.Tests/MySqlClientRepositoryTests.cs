using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.PriceComparison.Api.Common.Wrappers;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Api.Tests;

public class MySqlClientRepositoryTests : IClassFixture<MySqlIntegrationTestFactory>
{
    private readonly MySqlIntegrationTestFactory _factory;

    public MySqlClientRepositoryTests(MySqlIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateAuthClient()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
        return client;
    }

    [Fact]
    public async Task CreateAndRetrieveNaturalClient_PersistsInMySql()
    {
        var client = CreateAuthClient();

        var payload = new
        {
            name = "Carlos",
            middleName = (string?)null,
            lastName = "Ramirez",
            secondSurname = (string?)null,
            documentNumber = "55555555",
            electronicInvoiceEmail = "carlos@test.com",
            documentTypeId = 1,
            documentCountry = "CO"
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/client/natural", payload);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/v1/client/natural/55555555/document-number");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var body = await getResponse.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        var data = json.RootElement.GetProperty("data");
        Assert.Equal("Carlos", data.GetProperty("name").GetString());
        Assert.Equal("55555555", data.GetProperty("documentNumber").GetString());
    }

    [Fact]
    public async Task CreateAndRetrieveLegalClient_PersistsInMySql()
    {
        var client = CreateAuthClient();

        var payload = new
        {
            companyName = "Tech Solutions SAS",
            verificationDigit = 5,
            documentNumber = "800999000",
            electronicInvoiceEmail = "tech@test.com",
            vatResponsibleParty = true,
            selfRetainer = false,
            withholdingAgent = true,
            simpleTaxRegime = false,
            documentTypeId = 3,
            largeTaxpayer = false,
            documentCountry = "CO"
        };

        var createResponse = await client.PostAsJsonAsync("/api/v1/client/legal", payload);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/v1/client/legal/800999000/document-number");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var body = await getResponse.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        var data = json.RootElement.GetProperty("data");
        Assert.Equal("800999000", data.GetProperty("documentNumber").GetString());
    }

    [Fact]
    public async Task CreateNaturalClient_GetAll_ReturnsCreatedClient()
    {
        var client = CreateAuthClient();

        var payload = new
        {
            name = "Diana",
            lastName = "Martinez",
            documentNumber = "33334444",
            electronicInvoiceEmail = "diana@test.com",
            documentTypeId = 1,
            documentCountry = "CO"
        };
        await client.PostAsJsonAsync("/api/v1/client/natural", payload);

        var getAllResponse = await client.GetAsync("/api/v1/client/natural?pageNumber=1&pageSize=100");
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        var body = await getAllResponse.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.True(json.RootElement.GetProperty("success").GetBoolean());
        var data = json.RootElement.GetProperty("data");
        Assert.True(data.GetArrayLength() >= 1);

        var found = data.EnumerateArray().Any(d => d.GetProperty("documentNumber").GetString() == "33334444");
        Assert.True(found);
    }

    [Fact]
    public async Task GetNaturalClientById_NonExistent_ReturnsError()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/natural/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.False(json.RootElement.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GetAllDocumentTypes_ReturnsSeededData()
    {
        var client = CreateAuthClient();

        var response = await client.GetAsync("/api/v1/client/document-type");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.Equal("success", json.RootElement.GetProperty("status").GetString());
        Assert.True(json.RootElement.GetProperty("data").GetArrayLength() >= 1);
    }

    [Fact]
    public async Task CreateDuplicateNaturalClient_ReturnsOk_SecondTime()
    {
        var client = CreateAuthClient();

        var payload = new
        {
            name = "Elena",
            lastName = "Diaz",
            documentNumber = "11119999",
            electronicInvoiceEmail = "elena@test.com",
            documentTypeId = 1,
            documentCountry = "CO"
        };

        var first = await client.PostAsJsonAsync("/api/v1/client/natural", payload);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/v1/client/natural", payload);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
    }
}
