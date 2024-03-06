using Microsoft.Extensions.DependencyInjection;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Core.Callbacks;
using SteamTelegramBot.Core.Services;

namespace SteamTelegramBot.Core.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceSuffix = "Service";
    private const string CallbackSuffix = "Callback";

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var serviceTypes = GetAllTypes(typeof(BaseService), ServiceSuffix);
        
        services.RegisterImplementations(serviceTypes);

        return services;
    }

    public static IServiceCollection RegisterCallbacks(this IServiceCollection services)
    {
        var baseCallbackType = typeof(BaseCallback);
        var callbackTypes = GetAllTypes(baseCallbackType, CallbackSuffix);

        foreach (var callbackType in callbackTypes)
        {
            services.AddScoped(baseCallbackType, callbackType);
        }

        return services;
    }

    private static List<Type> GetAllTypes(Type baseType, string suffix)
        => baseType.Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith(suffix) && !x.IsAbstract && !x.IsInterface)
            .ToList();
}