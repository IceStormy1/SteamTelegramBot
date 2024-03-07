using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Core.Interfaces;

namespace SteamTelegramBot.Jobs.Jobs;

[DisallowConcurrentExecution]
internal class CheckingSteamAppsJob : IJob
{
    private const string JobName = nameof(CheckingSteamAppsJob);
    private const byte MaxUpdatedApplications = 60;

    private int _totalApplications;
    private int _totalUpdated;
    private int _totalSuccessfulUpdatedApplications;
    private int _totalApplicationsNotFound;

    private ISteamService _steamService;

    private readonly ILogger<CheckingSteamAppsJob> _logger;
    private readonly IServiceScopeFactory _scopeServiceProvider;
    private readonly ITelegramNotificationService _telegramNotificationService;
   
    public CheckingSteamAppsJob(
        ILogger<CheckingSteamAppsJob> logger,
        ISteamService steamService,
        IServiceScopeFactory scopeServiceProvider, 
        ITelegramNotificationService telegramNotificationService)
    {
        _logger = logger;
        _steamService = steamService;
        _scopeServiceProvider = scopeServiceProvider;
        _telegramNotificationService = telegramNotificationService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{ScheduleJob} - Start of the Steam applications update", JobName);
      
        var allApplications = await _steamService.GetAllSteamApps();

        _totalApplications = allApplications.Count;
        _logger.LogInformation("{ScheduleJob} - Total applications: {TotalApplications}", JobName, _totalApplications);

        while (true)
        {
            var updatedApplications = allApplications.Skip(_totalUpdated)
                .Take(MaxUpdatedApplications)
                .ToList();

            if (updatedApplications.Count == default)
                break;

            await AddOrUpdateSteamApplications(updatedApplications);
        }

        await _telegramNotificationService.NotifyUsersOfPriceDrop();

        _logger.LogInformation(
            "{ScheduleJob} - End of the Steam application update. " +
            "Total applications: {TotalUpdatedApplications}. " +
            "Applications not found: {ApplicationsNotFound}",
            JobName,
            _totalSuccessfulUpdatedApplications, 
            _totalApplicationsNotFound);
    }

    private async Task AddOrUpdateSteamApplications(IReadOnlyCollection<AppItemDto> updatedApplications)
    {
        var steamSuggestTasks = updatedApplications.Select(x => _steamService.GetSteamSuggests(x.Name));

        await using var scope = _scopeServiceProvider.CreateAsyncScope();
        _steamService = scope.ServiceProvider.GetRequiredService<ISteamService>();
        
        try
        {
            var steamSuggestResults = await Task.WhenAll(steamSuggestTasks);

            var updatedApplicationsIds = updatedApplications.Select(x => x.AppId).ToList();
            var foundedSteamApplications = CompareSuggestWithItems(steamSuggestResults, updatedApplicationsIds);

            await _steamService.AddOrUpdateSteamApplications(foundedSteamApplications);

            _totalUpdated += MaxUpdatedApplications;
            _totalApplicationsNotFound += updatedApplications.Count - foundedSteamApplications.Count;
            _totalSuccessfulUpdatedApplications += foundedSteamApplications.Count;

            _logger.LogInformation("{ScheduleJob} - Total applications {TotalApplications}; TotalUpdated: {TotalUpdated}; Updated: {TotalSuccessfulUpdatedApplications}; Not founded: {TotalNotFounded}",
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
            _logger.LogError(e, "An error occurred while updating steam applications");
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

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