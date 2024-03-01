using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Data.Repositories;

namespace SteamTelegramBot.Data.Extensions;

public static class ServiceCollectionExtensions
{
    private const string RepositorySuffix = "Repository";

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var serviceTypes = typeof(BaseRepository<>).Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith(RepositorySuffix) && !x.IsAbstract && !x.IsInterface)
            .ToList();

        return services.RegisterImplementations(serviceTypes);
    }

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