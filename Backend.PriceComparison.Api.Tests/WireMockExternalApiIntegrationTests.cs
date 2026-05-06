using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Backend.PriceComparison.Api.Tests;

public class WireMockExternalApiIntegrationTests : IDisposable
{
    private readonly WireMockServer _server;

    public WireMockExternalApiIntegrationTests()
    {
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 0,
            StartAdminInterface = true
        });
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }

    private string BaseUrl => _server.Urls[0];

    [Fact]
    public async Task WireMock_StubExternalApi_ReturnsMockedResponse()
    {
        _server
            .Given(Request.Create()
                .WithPath("/equivalent/pos")
                .WithParam("page", "1")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"
                {
                    ""data"": [
                        {
                            ""id"": ""eq-001"",
                            ""name"": ""Producto Mock"",
                            ""price"": 15000.00,
                            ""currency"": ""COP""
                        }
                    ],
                    ""meta"": {
                        ""page"": 1,
                        ""totalPages"": 1,
                        ""totalRecords"": 1
                    }
                }"));

        using var client = new HttpClient();
        var response = await client.GetAsync($"{BaseUrl}/equivalent/pos?page=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Producto Mock", content);
        Assert.Contains("\"price\": 15000.00", content);
    }

    [Fact]
    public async Task WireMock_StubErrorResponse_Returns500()
    {
        _server
            .Given(Request.Create()
                .WithPath("/equivalent/pos")
                .WithParam("state", "Error")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(500)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{ ""error"": ""Internal Server Error"", ""code"": ""PLEMSI_ERR_001"" }"));

        using var client = new HttpClient();
        var response = await client.GetAsync($"{BaseUrl}/equivalent/pos?state=Error");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("PLEMSI_ERR_001", content);
    }

    [Fact]
    public async Task WireMock_StubWithApiKeyAuth_ReturnsData()
    {
        _server
            .Given(Request.Create()
                .WithPath("/api/equivalent/pos")
                .WithHeader("X-API-Key", "test-api-key")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody(@"{ ""valid"": true }"));

        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/api/equivalent/pos");
        request.Headers.Add("X-API-Key", "test-api-key");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task WireMock_StubWithoutAuth_Returns404()
    {
        _server
            .Given(Request.Create()
                .WithPath("/api/secure/endpoint")
                .WithHeader("X-API-Key", "valid-key")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody(@"{ ""valid"": true }"));

        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/api/secure/endpoint");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task WireMock_StubPostEndpoint_ReturnsCreated()
    {
        _server
            .Given(Request.Create()
                .WithPath("/api/pos")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Location", "/api/pos/999")
                .WithBody(@"{ ""id"": 999, ""status"": ""created"" }"));

        using var client = new HttpClient();
        var content = new StringContent(@"{ ""name"": ""Test POS"" }", System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{BaseUrl}/api/pos", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task WireMock_StubWithDelay_SimulatesLatency()
    {
        _server
            .Given(Request.Create()
                .WithPath("/slow")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody(@"{ ""status"": ""ok"" }")
                .WithDelay(TimeSpan.FromMilliseconds(200)));

        using var client = new HttpClient();
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var response = await client.GetAsync($"{BaseUrl}/slow");
        sw.Stop();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(sw.ElapsedMilliseconds >= 150);
    }
}
