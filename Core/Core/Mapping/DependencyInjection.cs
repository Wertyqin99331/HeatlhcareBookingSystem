using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Mapping;

public static class DependencyInjection
{
    public static IServiceCollection AddMappingServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMapster();
        TypeAdapterConfig.GlobalSettings.Scan(assemblies);

        return services;
    }
}