using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using SteamTelegramBot.Abstractions.Models.Applications;
using SteamTelegramBot.Core.Interfaces;

namespace SteamTelegramBot.Jobs.Jobs;

/// <summary>
/// Represents a Quartz job for checking and updating Steam applications.
/// </summary>
[DisallowConcurrentExecution]
internal class CheckingSteamAppsJob(
    ILogger<CheckingSteamAppsJob> logger,
    ISteamService steamService,
    IServiceScopeFactory scopeServiceProvider,
    ITelegramNotificationService telegramNotificationService,
    IUserAppTrackingService userAppTrackingService)
    : IJob
{
    private const string JobName = nameof(CheckingSteamAppsJob);
    private const byte MaxUpdatedApplications = 60;

    private int _totalApplications;
    private int _totalUpdated;
    private int _totalSuccessfulUpdatedApplications;
    private int _totalApplicationsNotFound;

    /// <summary>
    /// Executes the Steam applications update job.
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{ScheduleJob} - Start of the Steam applications update", JobName);
      
        var allApplications = await steamService.GetAllSteamApps();

        _totalApplications = allApplications.Count;
        logger.LogInformation("{ScheduleJob} - Total applications: {TotalApplications}", JobName, _totalApplications);

        await UpdateApplications(allApplications);

        logger.LogInformation(
            "{ScheduleJob} - End of the Steam application update. " +
            "Total applications: {TotalUpdatedApplications}. " +
            "Applications not found: {ApplicationsNotFound}",
            JobName,
            _totalSuccessfulUpdatedApplications, 
            _totalApplicationsNotFound);
    }

    /// <summary>
    /// Updates the tracked applications.
    /// </summary>
    private async Task UpdateApplications(IReadOnlyCollection<AppItemDto> allApplications)
    {
        var updatedAppsIds = await UpdateTrackedApplications(allApplications);
        allApplications = allApplications.Where(x => !updatedAppsIds.Contains(x.AppId)).ToList();

        while (true)
        {
            var updatedApplications = allApplications.Skip(_totalUpdated)
                .Take(MaxUpdatedApplications)
                .ToList();

            if (updatedApplications.Count == default)
                break;

            await AddOrUpdateSteamApplications(updatedApplications);
        }
    }

    /// <summary>
    /// Updates the tracked applications.
    /// </summary>
    private async Task<List<int>> UpdateTrackedApplications(IReadOnlyCollection<AppItemDto> allApplications)
    {
        var updatedAppsIds = new List<int>();

        while (true)
        {
            var trackedApplicationIds = 
                await userAppTrackingService.GetUsersTrackedAppsIds(limit: MaxUpdatedApplications, offset: updatedAppsIds.Count);

            if (trackedApplicationIds.Count == default)
                break;

            var trackedItems = allApplications.Where(x => trackedApplicationIds.Contains(x.AppId)).ToList();
            await AddOrUpdateSteamApplications(trackedItems);

            updatedAppsIds.AddRange(trackedApplicationIds);
        }

        await telegramNotificationService.NotifyUsersOfPriceDrop(updatedAppsIds);

        return updatedAppsIds;
    }

    /// <summary>
    /// Adds or updates the Steam applications.
    /// </summary>
    private async Task AddOrUpdateSteamApplications(IReadOnlyCollection<AppItemDto> updatedApplications)
    {
        await using var scope = scopeServiceProvider.CreateAsyncScope();
        steamService = scope.ServiceProvider.GetRequiredService<ISteamService>();

        try
        {
            var foundedSteamApplications = await FindAndUpdateSteamApplications(updatedApplications);

            _totalApplicationsNotFound += updatedApplications.Count - foundedSteamApplications.Count;
            _totalSuccessfulUpdatedApplications += foundedSteamApplications.Count;
            _totalUpdated = _totalSuccessfulUpdatedApplications + _totalApplicationsNotFound;

            logger.LogInformation("{ScheduleJob} - Total applications {TotalApplications}; TotalUpdated: {TotalUpdated}; Updated: {TotalSuccessfulUpdatedApplications}; Not founded: {TotalNotFounded}",
                JobName,
                _totalApplications,
                _totalUpdated,
                _totalSuccessfulUpdatedApplications,
                _totalApplicationsNotFound);

            // Steam Api throws error if spam requests, so delay in 5 seconds between batches of requests
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating steam applications");
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    /// <summary>
    /// Finds and updates the Steam applications.
    /// </summary>
    private async Task<List<SteamSuggestItem>> FindAndUpdateSteamApplications(IReadOnlyCollection<AppItemDto> applications)
    {
        var steamSuggestResults = await FindSteamApplications(applications);

        var updatedApplicationsIds = applications.Select(x => x.AppId).ToList();
        var foundedSteamApplications = CompareSuggestWithItems(steamSuggestResults, updatedApplicationsIds);

        await steamService.AddOrUpdateSteamApplications(foundedSteamApplications);

        return foundedSteamApplications;
    }

    /// <summary>
    /// Finds the Steam applications.
    /// </summary>
    private async Task<IReadOnlyCollection<SteamSuggestItem>[]> FindSteamApplications(IEnumerable<AppItemDto> applications)
    {
        var steamSuggestTasks = applications.Select(x => steamService.GetSteamSuggests(x.Name));
        var steamSuggestResults = await Task.WhenAll(steamSuggestTasks);

        return steamSuggestResults;
    }

    /// <summary>
    /// Compares the suggest items with the updated application IDs.
    /// </summary>
    private static List<SteamSuggestItem> CompareSuggestWithItems(
        IEnumerable<IReadOnlyCollection<SteamSuggestItem>> steamSuggestResults,
        ICollection<int> updatedAppsIds)
    {
        return steamSuggestResults
            .SelectMany(x => x)
            .Where(x => updatedAppsIds.Contains(x.AppId))
            .DistinctBy(x => x.AppId)
            .OrderBy(x => x.AppId)
            .ToList();
    }
}