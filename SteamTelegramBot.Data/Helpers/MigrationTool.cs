using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SteamTelegramBot.Data.Helpers;

/// <summary>
/// Represents a migration tool for applying database migrations.
/// </summary>
public sealed class MigrationTool
{
    private readonly IServiceProvider _rootServiceProvider;
    private readonly ILogger<MigrationTool> _logger;

    public MigrationTool(IServiceProvider rootServiceProvider)
    {
        _rootServiceProvider = rootServiceProvider;
        _logger = rootServiceProvider.GetRequiredService<ILogger<MigrationTool>>();
    }

    /// <summary>
    /// Executes the migration process using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public static void Execute(IServiceProvider serviceProvider)
        => new MigrationTool(serviceProvider).Migrate();

    /// <summary>
    /// Performs the migration process.
    /// </summary>
    private void Migrate()
    {
        _logger.LogInformation("Creating scope...");

        try
        {
            using var scope = _rootServiceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<SteamTelegramBotDbContext>();

            _logger.LogInformation("Migrating DbContext '{DbContext}'...", dbContext.GetType());

            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(2));
            dbContext.Database.Migrate();
            dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

            _logger.LogInformation("Migrate for DbContext '{DbContext}' is complete", dbContext.GetType());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while applying migration");
            throw;
        }

        _logger.LogInformation("Migrations are complete");
    }
}