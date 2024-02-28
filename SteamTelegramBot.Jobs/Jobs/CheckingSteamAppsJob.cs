using Microsoft.Extensions.Logging;
using Quartz;
using SteamTelegramBot.Abstractions.Services;

namespace SteamTelegramBot.Jobs.Jobs;

[DisallowConcurrentExecution]
internal class CheckingSteamAppsJob : IJob
{
    private const string JobName = nameof(CheckingSteamAppsJob);
    private const byte MaxUpdatedApplications = 60;

    private readonly ILogger<CheckingSteamAppsJob> _logger;
    private readonly ISteamService _steamService;

    public CheckingSteamAppsJob(
        ILogger<CheckingSteamAppsJob> logger,
        ISteamService steamService
    )
    {
        _logger = logger;
        _steamService = steamService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{ScheduleJob} - Start of the Steam application update", JobName);

        var allApplications = await _steamService.GetAllSteamApps();
        _logger.LogInformation("{ScheduleJob} - Total applications: {TotalApplications}", JobName, allApplications.Count);

        var totalUpdatedApplications = 0;
        var totalApplicationsNotFound = 0;

        while (true)
        {
            var updatedApplications = allApplications.Skip(totalUpdatedApplications)
                .Take(MaxUpdatedApplications)
                .ToList();

            if (updatedApplications.Count == default)
                break;
         
            var tasks = updatedApplications.Select(x => _steamService.GetSteamSuggests(x.Name));

            try
            {
                var results = await Task.WhenAll(tasks);

                totalApplicationsNotFound += results.Count(x => x is null);
                totalUpdatedApplications += MaxUpdatedApplications;

                _logger.LogInformation("{ScheduleJob} - Total {TotalApplications}; Updated: {TotalUpdated}", JobName, allApplications.Count, totalUpdatedApplications);

                await Task.Delay(TimeSpan.FromSeconds(5));

                // TODO: Добавить проверку на AppId из GetAllSteamApps и GetSteamSuggests
                // TODO: Чтобы не проходиться каждый раз по всему списку: Искать в базе по appid и price - для обновления. 
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occ");
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