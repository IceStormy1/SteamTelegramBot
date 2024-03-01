using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using SteamTelegramBot.Core.Interfaces;

namespace SteamTelegramBot.Jobs.Jobs;

[DisallowConcurrentExecution]
internal class CheckingSteamAppsJob : IJob
{
    private const string JobName = nameof(CheckingSteamAppsJob);
    private const byte MaxUpdatedApplications = 60;

    private ISteamService _steamService;

    private readonly ILogger<CheckingSteamAppsJob> _logger;
    private readonly IServiceScopeFactory _scopeServiceProvider;

    public CheckingSteamAppsJob(
        ILogger<CheckingSteamAppsJob> logger,
        ISteamService steamService,
        IServiceScopeFactory scopeServiceProvider)
    {
        _logger = logger;
        _steamService = steamService;
        _scopeServiceProvider = scopeServiceProvider;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{ScheduleJob} - Start of the Steam applications update", JobName);

        var allApplications = await _steamService.GetAllSteamApps();
        _logger.LogInformation("{ScheduleJob} - Total applications: {TotalApplications}", JobName, allApplications.Count);

        var total = 0;
        var totalUpdatedApplications = 0;
        var totalApplicationsNotFound = 0;

        while (true)
        {
            var updatedApplications = allApplications.Skip(total)
                .Take(MaxUpdatedApplications)
                .ToList();

            if (updatedApplications.Count == default)
                break;
         
            var steamSuggestTasks = updatedApplications.Select(x => _steamService.GetSteamSuggests(x.Name));

            await using var scope = _scopeServiceProvider.CreateAsyncScope();
            _steamService = scope.ServiceProvider.GetRequiredService<ISteamService>();

            try
            {
                var steamSuggestResults = await Task.WhenAll(steamSuggestTasks);

                var updatedApplicationsIds = updatedApplications.Select(x => x.AppId).ToList();
                var foundedSteamApplications = steamSuggestResults
                    .SelectMany(x => x)
                    .Where(x => updatedApplicationsIds.Contains(x.AppId))
                    .DistinctBy(x => x.AppId)
                    .OrderBy(x => x.AppId)
                    .ToList();

                await _steamService.AddOrUpdateSteamApplications(foundedSteamApplications);

                total += MaxUpdatedApplications;
                totalApplicationsNotFound += updatedApplications.Count - foundedSteamApplications.Count;
                totalUpdatedApplications += foundedSteamApplications.Count;

                _logger.LogInformation("{ScheduleJob} - Total {TotalApplications}; Updated: {TotalUpdated}; Not founded: {TotalNotFounded}", 
                    JobName, 
                    allApplications.Count, 
                    totalUpdatedApplications,
                    totalApplicationsNotFound);

                // Steam Api throws error if spam requests, so delay in 5 seconds between batches of requests
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating steam applications");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        _logger.LogInformation(
            "{ScheduleJob} - End of the Steam application update. " +
            "Total applications: {TotalUpdatedApplications}. " +
            "Applications not found: {ApplicationsNotFound}",
            JobName,
            totalUpdatedApplications, 
            totalApplicationsNotFound);
    }
}