using Microsoft.Extensions.DependencyInjection;
using SteamTelegramBot.Abstractions.Services;
using SteamTelegramBot.Core.Services;

namespace SteamTelegramBot.Core.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceSuffix = "Service";

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var serviceTypes = typeof(BaseService).Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith(ServiceSuffix) && !x.IsAbstract && !x.IsInterface)
            .ToList();

        services.RegisterImplementations(serviceTypes);

        return services.AddScoped<ITelegramHandler, TelegramHandler>();
    }

    private static IServiceCollection RegisterImplementations(this IServiceCollection services, IEnumerable<Type> implementationTypes)
    {
        foreach (var implementationType in implementationTypes)
        {
            var serviceInterface = implementationType.GetInterfaces();

            if (serviceInterface.Length > 0)
            {
                foreach (var @interface in serviceInterface)
                {
                    services.AddScoped(@interface, implementationType);
                }
            }
            else
            {
                services.AddScoped(implementationType);
            }
        }

        return services;
    }
}