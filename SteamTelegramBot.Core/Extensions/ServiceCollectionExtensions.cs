using Microsoft.Extensions.DependencyInjection;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Core.Callbacks;
using SteamTelegramBot.Core.Commands;
using SteamTelegramBot.Core.Services;

namespace SteamTelegramBot.Core.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceSuffix = "Service";
    private const string CallbackSuffix = "Callback";
    private const string CommandSuffix = "Command";

    /// <summary>
    /// Register all services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var serviceTypes = GetAllTypes(typeof(BaseService), ServiceSuffix);
        
        services.RegisterImplementations(serviceTypes);

        return services;
    }

    /// <summary>
    /// Register all callbacks
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterCallbacks(this IServiceCollection services) 
        => services.RegisterTypesWithBase<BaseCallback>(CallbackSuffix);

    /// <summary>
    /// Register all commands
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterCommands(this IServiceCollection services)
        => services.RegisterTypesWithBase<BaseCommand>(CommandSuffix);

    private static IServiceCollection RegisterTypesWithBase<TBase>(this IServiceCollection services, string suffix)
    {
        var baseType = typeof(TBase);

        var commandTypes = GetAllTypes(baseType, suffix);

        foreach (var callbackType in commandTypes)
            services.AddScoped(baseType, callbackType);
        
        return services;
    }

    private static List<Type> GetAllTypes(Type baseType, string suffix)
        => baseType.Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith(suffix) && !x.IsAbstract && !x.IsInterface)
            .ToList();
}