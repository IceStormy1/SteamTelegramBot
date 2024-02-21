using SteamTelegramBot.Abstractions.Configurations;

namespace SteamTelegramBot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services
                .Configure<BotConfiguration>(configuration.GetSection(nameof(BotConfiguration)))
            ;
    }
}