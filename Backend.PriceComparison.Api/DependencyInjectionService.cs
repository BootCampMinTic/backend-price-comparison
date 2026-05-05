using Backend.PriceComparison.Api.Common.Configurations;

namespace Backend.PriceComparison.Api;

public static class DependencyInjectionService
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.AddFluentValidationServices();
        
        // OpenAPI is configured in Program.cs
        
        return services;
    }
}

