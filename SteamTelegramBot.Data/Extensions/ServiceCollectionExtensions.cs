using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SteamTelegramBot.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, bool enableSensitiveData = false)
    {
        return services.AddDbContextPool<SteamTelegramBotDbContext>(action =>
        {
            action.UseNpgsql(configuration.GetConnectionString("SteamConnectionString"),
                options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

            if (enableSensitiveData)
                action.EnableSensitiveDataLogging();
        });
    }
}