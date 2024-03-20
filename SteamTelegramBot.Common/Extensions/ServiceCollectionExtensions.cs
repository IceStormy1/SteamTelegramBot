using Microsoft.Extensions.DependencyInjection;

namespace SteamTelegramBot.Common.Extensions;

/// <summary>
/// Provides extension methods for registering implementations in the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers implementations of service interfaces in the service collection.
    /// </summary>
    /// <param name="services">The service collection to register implementations with.</param>
    /// <param name="implementationTypes">The types representing implementations of service interfaces.</param>
    /// <returns>The modified service collection with implementations registered.</returns>
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