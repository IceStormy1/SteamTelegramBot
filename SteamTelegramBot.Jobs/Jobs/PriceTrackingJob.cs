using Microsoft.Extensions.Logging;
using Quartz;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Jobs.Jobs;

[DisallowConcurrentExecution]
public sealed class PriceTrackingJob(ILogger<PriceTrackingJob> logger,
    ISteamService steamService,
    ICheckingSteamAppsService checkingSteamAppsService) : IJob
{
    private const string JobName = nameof(PriceTrackingJob);

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{ScheduleJob} - Start checking tracked applications", JobName);
        var allApplications = await steamService.GetAllSteamApps();

        var updatedApplicationIds = await checkingSteamAppsService.UpdateTrackedApplications(allApplications);

        logger.LogInformation(
            "{ScheduleJob} - End checking tracked applications. Updated: {TotalTrackedApplications}", JobName, updatedApplicationIds.Count);
    }
}