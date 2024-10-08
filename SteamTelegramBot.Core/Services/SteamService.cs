﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models.Applications;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Core.Services;

internal sealed class SteamService(
    IMapper mapper,
    ILogger<SteamService> logger,
    ISteamWebApiClient steamWebApiClient,
    IStoreSteamPoweredClient storeSteamPoweredClient,
    ISteamAppRepository steamAppRepository)
    : BaseService(mapper, logger), ISteamService
{
    private const string NameHtmlTag = "match_name";
    private const string PriceHtmlTag = "match_subtitle";

    public async Task<IReadOnlyCollection<AppItemDto>> GetAllSteamApps(bool ascending = false)
    {
        var allSteamApps = await steamWebApiClient.GetAllApps();

        if (!allSteamApps.IsSuccessStatusCode || allSteamApps.Content is null)
            throw new SteamException(allSteamApps.Error, "An error occurred while receiving all applications");

        var notEmptyApplications = allSteamApps.Content.AppList.Apps
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            ;

        var orderedApplications = ascending
            ? notEmptyApplications.OrderBy(x => x.AppId)
            : notEmptyApplications.OrderByDescending(x => x.AppId);

        return Mapper.Map<List<AppItemDto>>(orderedApplications);
    }

    public async Task<IReadOnlyCollection<SteamSuggestItem>> GetSteamSuggests(string steamAppName, bool filterByExistingApps = false)
    {
        var steamSuggestsHtml = await storeSteamPoweredClient.GetSuggests(steamAppName);

        var steamSuggestItems = steamSuggestsHtml.Split("ds_collapse_flag")
            .Where(s => s.Contains(NameHtmlTag) && s.Contains(PriceHtmlTag))
            .Select(s => new SteamSuggestItem(s))
            .ToList();

        if (!filterByExistingApps || steamSuggestItems.Count == default)
            return steamSuggestItems;

        var steamSuggestItemsIds = steamSuggestItems.Select(x => x.AppId).ToList();
        var existingAppsIds = await steamAppRepository.CheckSteamApplicationsByIds(steamSuggestItemsIds);

        return steamSuggestItems.Where(x => existingAppsIds.Contains(x.AppId)).ToList();
    }

    public async Task AddOrUpdateSteamApplications(List<SteamSuggestItem> steamSuggestItems)
    {
        if (steamSuggestItems.Count == default)
            return;

        var steamSuggestIds = steamSuggestItems.Select(item => item.AppId).ToList();
        var existingApps = await steamAppRepository.GetSteamApplicationsByIds(steamSuggestIds);
        var existingAppsIds = existingApps.Select(entity => entity.SteamAppId).ToList();
        var newApps = steamSuggestItems.Where(item => !existingAppsIds.Contains(item.AppId)).ToList();

        var addedApps = await AddSteamApplications(newApps);
        existingApps.AddRange(addedApps);

        await UpdateSteamApplications(steamSuggestItems, existingApps);
    }

    private async Task<List<SteamAppEntity>> AddSteamApplications(IReadOnlyCollection<SteamSuggestItem> newSteamApps)
    {
        if(newSteamApps.Count == default)
            return new List<SteamAppEntity>();

        var newEntities = Mapper.Map<List<SteamAppEntity>>(newSteamApps);
        AddNewPrice(newSteamApps, newEntities);

        await steamAppRepository.AddRange(newEntities);

        return newEntities;
    }

    private async Task UpdateSteamApplications(List<SteamSuggestItem> steamSuggestItems, List<SteamAppEntity> existingSteamApps)
    {
        if (existingSteamApps.Count == default)
            return;

        var appsWithDifferentPrice = existingSteamApps
            .Where(entity => steamSuggestItems.Any(item => item.AppId == entity.SteamAppId 
                                                           && item.Price != entity.PriceHistory.MaxBy(x=>x.Version)!.Price
                                                           ))
            .ToList();

        AddNewPrice(steamSuggestItems, appsWithDifferentPrice);

        await steamAppRepository.UpdateRange(
            source: steamSuggestItems, 
            entities: existingSteamApps, 
            find: (src, target) => target.First(t => t.SteamAppId == src.AppId));
    }

    private static void AddNewPrice(IReadOnlyCollection<SteamSuggestItem> steamSuggestItems, List<SteamAppEntity> steamAppEntities)
    {
        foreach (var steamAppEntity in steamAppEntities)
        {
            var steamSuggestItem = steamSuggestItems.First(x => x.AppId == steamAppEntity.SteamAppId);
            var previousVersion = steamAppEntity.PriceHistory.MaxBy(x => x.Version);
            var telegramNotifications = new List<TelegramNotificationEntity>();
                
            if(steamSuggestItem.Price < previousVersion?.Price)
            {
                telegramNotifications = steamAppEntity.TrackedUsers
                    .Select(x => new TelegramNotificationEntity { UserAppTrackingId = x.Id })
                    .ToList();
            }

            var priceHistory = new SteamAppPriceHistoryEntity
            {
                Price = steamSuggestItem.Price,
                PriceType = steamSuggestItem.PriceType,
                Version = (previousVersion?.Version ?? default) + 1,
                TelegramNotifications = telegramNotifications
            };

            steamAppEntity.PriceHistory.Add(priceHistory);
        }
    }
}