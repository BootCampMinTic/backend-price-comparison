using System.Net;
using System.Text.Json;

namespace Backend.PriceComparison.Api.Tests;

public class HealthEndpointsIntegrationTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory = factory;

    [Fact]
    public async Task GetLiveStatus_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);

        Assert.Equal("Alive", json.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task GetLive_WithoutAuthToken_Succeeds()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
