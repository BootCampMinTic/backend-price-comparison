using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Backend.PriceComparison.Application.Common.Behaviors;
using Backend.PriceComparison.Application.Client.Mappers;
using Backend.PriceComparison.Application.Client.Services;
using System.Reflection;

namespace Backend.PriceComparison.Application;

public static class DependencyInjectionService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        #region Mappers
        var mapper = new MapperConfiguration(config =>
        {
            config.AddProfile(new ClientProfile());
        });
        services.AddSingleton(mapper.CreateMapper());
        #endregion

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>)
        );

        #region Client Services
        services.AddScoped<IClientQueryService, ClientQueryService>();
        services.AddScoped<IClientCommandService, ClientCommandService>();
        #endregion

        return services;
    }
}