using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Adapter;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Client.Repositories;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Mock;
using Testcontainers.MySql;

namespace Backend.PriceComparison.Api.Tests;

public class MySqlIntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mysql = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    private string _connectionString = string.Empty;

    public HttpClient HttpClient { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            RemoveAll(services, typeof(ICacheService));
            RemoveAll(services, typeof(IClientRepository));
            RemoveAll(services, typeof(IDocumentTypeRepository));
            RemoveAll(services, typeof(IMessageProvider));
            RemoveAll(services, typeof(ClientDbContext));
            RemoveAll(services, typeof(DbContextOptions<ClientDbContext>));
            RemoveAll(services, typeof(DbContextOptions));
            RemoveAll(services, typeof(IDbContextOptionsConfiguration<ClientDbContext>));
            RemoveAll(services, typeof(IDbContextFactory<ClientDbContext>));

            services.AddSingleton<ICacheService, InMemoryCacheService>();
            services.AddSingleton<IMessageProvider, MessageProvider>();

            services.AddDbContext<ClientDbContext>(options =>
                options.UseMySql(_connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        });
    }

    private static void RemoveAll(IServiceCollection services, Type serviceType)
    {
        var descriptors = services.Where(d => d.ServiceType == serviceType).ToList();
        foreach (var d in descriptors)
            services.Remove(d);
    }

    public async Task InitializeAsync()
    {
        await _mysql.StartAsync();
        _connectionString = _mysql.GetConnectionString();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ClientDbContext>();
        await db.Database.EnsureCreatedAsync();

        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        HttpClient?.Dispose();
        await _mysql.DisposeAsync();
        await base.DisposeAsync();
    }
}
