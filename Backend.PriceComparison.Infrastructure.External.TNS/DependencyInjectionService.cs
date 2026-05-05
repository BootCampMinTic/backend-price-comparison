using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.PriceComparison.Infrastructure.External.TNS
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddExternalTns(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
