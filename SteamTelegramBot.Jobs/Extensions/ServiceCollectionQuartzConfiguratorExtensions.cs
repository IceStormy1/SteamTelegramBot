using Microsoft.Extensions.Configuration;
using Quartz;

namespace SteamTelegramBot.Jobs.Extensions;

public static class ServiceCollectionQuartzConfiguratorExtensions
{
    /// <summary>
    /// Adds a job and trigger based on the specified job type and configuration.
    /// </summary>
    /// <typeparam name="T">The type of job to add.</typeparam>
    /// <param name="quartz">The Quartz configurator.</param>
    /// <param name="config">The configuration containing the job schedule.</param>
    /// <exception cref="InvalidOperationException">Thrown when unable to determine job name.</exception>
    /// <exception cref="ArgumentException">Thrown when no Quartz.NET Cron schedule found for job in configuration.</exception>
    public static void AddJobAndTrigger<T>(
        this IServiceCollectionQuartzConfigurator quartz,
        IConfiguration config)
        where T : IJob
    {
        var jobName = typeof(T).FullName;

        if (string.IsNullOrWhiteSpace(jobName))
            throw new InvalidOperationException("Unable to determine job name");

        var cronSchedule = config[jobName];

        if (string.IsNullOrEmpty(cronSchedule))
            throw new ArgumentException($"No Quartz.NET Cron schedule found for job in configuration at {jobName}");

        var jobKey = new JobKey(jobName);
        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithCronSchedule(cronSchedule));
    }
}