using Microsoft.Extensions.Configuration;
using Quartz;

namespace SteamTelegramBot.Jobs.Extensions;

public static class ServiceCollectionQuartzConfiguratorExtensions
{
    /// <summary>
    /// Add a job and trigger
    /// </summary>
    public static void AddJobAndTrigger<T>(
        this IServiceCollectionQuartzConfigurator quartz,
        IConfiguration config)
        where T : IJob
    {
        // Use the name of the IJob as the appsettings.json key
        var jobName = typeof(T).FullName;

        if (string.IsNullOrWhiteSpace(jobName))
        {
            throw new InvalidOperationException("Unable to determine job name");
        }

        // Try and load the schedule from configuration
        var cronSchedule = config[jobName];

        if (string.IsNullOrEmpty(cronSchedule))
        {
            throw new ArgumentException($"No Quartz.NET Cron schedule found for job in configuration at {jobName}");
        }

        // register the job
        var jobKey = new JobKey(jobName);
        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithCronSchedule(cronSchedule));
    }
}