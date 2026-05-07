using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;
using WireMock.Server;
using WireMock.Settings;

namespace Backend.PriceComparison.Api.Tests;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly WireMockServer _wireMock;

    public HttpClient HttpClient { get; private set; } = null!;
    public WireMockServer WireMock => _wireMock;
    public string WireMockBaseUrl => _wireMock.Urls[0];

    public IntegrationTestFactory()
    {
        _wireMock = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 0,
            StartAdminInterface = true
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        var baseUrl = WireMockBaseUrl;

        builder.ConfigureServices(services =>
        {
            var cacheDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICacheService));
            if (cacheDescriptor is not null) services.Remove(cacheDescriptor);
            services.AddSingleton<ICacheService, InMemoryCacheService>();
        });
    }

    public async Task InitializeAsync()
    {
        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        HttpClient?.Dispose();
        _wireMock.Stop();
        _wireMock.Dispose();
        await base.DisposeAsync();
    }
}
