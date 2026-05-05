using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Backend.PriceComparison.Domain.Ports;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Adapter;
using Backend.PriceComparison.Domain.ClientPos.DomainServices;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Client.DomainServices;
using Backend.PriceComparison.Infraestructure.Persistence.Mysql.Context;

namespace Backend.PriceComparison.Infraestructure.Persistence.Mysql;

public static class DependencyInjectionService
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION") ?? configuration.GetConnectionString("MysqlConnection");
        services.AddDbContext<ClientDbContext>(
            options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
        ));
        services.AddTransient<IMessageProvider, MessageProvider>();
        services.AddScoped<IClientDomainService, ClientDomainService>();
        services.AddScoped<IDocumentTypeDomainService, DocumentTypeDomainService>();
        
        return services;
    }
}
