﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using SteamTelegramBot.Abstractions.Models.Applications;
using SteamTelegramBot.Core.Interfaces;

namespace SteamTelegramBot.Core.Services;

internal sealed class CheckingSteamAppsService(
    IMapper mapper,
    ILogger<CheckingSteamAppsService> logger,
    IServiceScopeFactory scopeServiceProvider,
    ITelegramNotificationService telegramNotificationService,
    IUserAppTrackingService userAppTrackingService)
    : BaseService(mapper, logger), ICheckingSteamAppsService
{
    private const string ServiceName = nameof(CheckingSteamAppsService);
    private const byte MaxUpdatedApplications = 60;

    private int _totalUpdated;
    private int _totalSuccessfulUpdatedApplications;
    private int _totalApplicationsNotFound;
    private int _totalApplications;

    private ISteamService _steamService;

    /// <summary>
    /// Updates the tracked applications.
    /// </summary>
    public async Task<(int TotalSuccessfulUpdatedApplications, int TotalApplicationsNotFound)> UpdateApplications(IReadOnlyCollection<AppItemDto> allApplications)
    {
        _totalApplications = allApplications.Count;
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

        return (_totalSuccessfulUpdatedApplications, _totalApplicationsNotFound);
    }

    /// <summary>
    /// Updates the tracked applications.
    /// </summary>
    public async Task<List<int>> UpdateTrackedApplications(IReadOnlyCollection<AppItemDto> allApplications)
    {
        var updatedAppsIds = new List<int>();

        while (true)
        {
            var trackedApplicationIds =
                await userAppTrackingService.GetUsersTrackedAppsIds(limit: MaxUpdatedApplications,
                    offset: updatedAppsIds.Count);

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
        _steamService = scope.ServiceProvider.GetRequiredService<ISteamService>();

        try
        {
            var foundedSteamApplications = await FindAndUpdateSteamApplications(updatedApplications);

            _totalApplicationsNotFound += updatedApplications.Count - foundedSteamApplications.Count;
            _totalSuccessfulUpdatedApplications += foundedSteamApplications.Count;
            _totalUpdated = _totalSuccessfulUpdatedApplications + _totalApplicationsNotFound;

            logger.LogInformation(
                "{Service} - Total applications {TotalApplications}; TotalUpdated: {TotalUpdated}; Updated: {TotalSuccessfulUpdatedApplications}; Not founded: {TotalNotFounded}",
                ServiceName,
                _totalApplications,
                _totalUpdated,
                _totalSuccessfulUpdatedApplications,
                _totalApplicationsNotFound);

            // Steam Api throws error if spam requests, so delay in 15 seconds between batches of requests
            await Task.Delay(TimeSpan.FromSeconds(15));
        }
        catch (ApiException e)
        {
            logger.LogError(e, "{Service} - An error occurred while updating steam applications. Content: {Content}", ServiceName, e.Content);
            throw;
        }
    }

    /// <summary>
    /// Finds and updates the Steam applications.
    /// </summary>
    private async Task<List<SteamSuggestItem>> FindAndUpdateSteamApplications(
        IReadOnlyCollection<AppItemDto> applications)
    {
        var steamSuggestResults = await FindSteamApplications(applications);

        var updatedApplicationsIds = applications.Select(x => x.AppId).ToList();
        var foundedSteamApplications = CompareSuggestWithItems(steamSuggestResults, updatedApplicationsIds);

        await _steamService.AddOrUpdateSteamApplications(foundedSteamApplications);

        return foundedSteamApplications;
    }

    /// <summary>
    /// Finds the Steam applications.
    /// </summary>
    private async Task<IReadOnlyCollection<SteamSuggestItem>[]> FindSteamApplications(
        IEnumerable<AppItemDto> applications)
    {
        var steamSuggestTasks = applications.Select(x => _steamService.GetSteamSuggests(x.Name));
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