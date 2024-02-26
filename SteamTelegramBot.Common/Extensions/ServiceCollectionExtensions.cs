using Microsoft.Extensions.DependencyInjection;

namespace SteamTelegramBot.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterImplementations(this IServiceCollection services, IEnumerable<Type> implementationTypes)
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