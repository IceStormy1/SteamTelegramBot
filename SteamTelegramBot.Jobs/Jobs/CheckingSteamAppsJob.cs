using Microsoft.Extensions.Logging;
using Quartz;
using SteamTelegramBot.Core.Interfaces;

namespace SteamTelegramBot.Jobs.Jobs;

/// <summary>
/// Represents a Quartz job for checking and updating Steam applications.
/// </summary>
[DisallowConcurrentExecution]
internal class CheckingSteamAppsJob(
    ILogger<CheckingSteamAppsJob> logger,
    ISteamService steamService,
    ICheckingSteamAppsService checkingSteamAppsService)
    : IJob
{
    private const string JobName = nameof(CheckingSteamAppsJob);

    /// <summary>
    /// Executes the Steam applications update job.
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{ScheduleJob} - Start of the Steam applications update", JobName);
      
        var allApplications = await steamService.GetAllSteamApps(ascending: false);

        logger.LogInformation("{ScheduleJob} - Total applications: {TotalApplications}", JobName, allApplications.Count);

        var (totalSuccessfulUpdatedApplications, totalApplicationsNotFound) = await checkingSteamAppsService.UpdateApplications(allApplications);

        logger.LogInformation(
            "{ScheduleJob} - End of the Steam application update. " +
            "Total applications: {TotalUpdatedApplications}. " +
            "Applications not found: {ApplicationsNotFound}",
            JobName,
            totalSuccessfulUpdatedApplications,
            totalApplicationsNotFound);
    }
}