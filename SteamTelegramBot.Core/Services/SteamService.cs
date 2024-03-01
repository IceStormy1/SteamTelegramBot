using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Core.Services;

internal sealed class SteamService : BaseService, ISteamService
{
    private const string NameHtmlTag = "match_name";
    private const string PriceHtmlTag = "match_subtitle";

    private readonly ISteamWebApiClient _steamWebApiClient;
    private readonly IStoreSteamPoweredClient _storeSteamPoweredClient;
    private readonly ISteamAppRepository _steamAppRepository;
    private readonly ITelegramNotificationService _telegramNotificationService;

    public SteamService(
        IMapper mapper, 
        ILogger<BaseService> logger,
        ISteamWebApiClient steamWebApiClient,
        IStoreSteamPoweredClient storeSteamPoweredClient, 
        ISteamAppRepository steamAppRepository,
        ITelegramNotificationService telegramNotificationService) : base(mapper, logger)
    {
        _steamWebApiClient = steamWebApiClient;
        _storeSteamPoweredClient = storeSteamPoweredClient;
        _steamAppRepository = steamAppRepository;
        _telegramNotificationService = telegramNotificationService;
    }

    public async Task<IReadOnlyCollection<AppItemDto>> GetAllSteamApps()
    {
        var allSteamApps = await _steamWebApiClient.GetAllApps();

        if (!allSteamApps.IsSuccessStatusCode || allSteamApps.Content is null)
            throw new SteamException(allSteamApps.Error, "An error occurred while receiving all applications");

        var orderedApplications = allSteamApps.Content.AppList.Apps
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .OrderBy(x => x.AppId);

        return Mapper.Map<List<AppItemDto>>(orderedApplications);
    }

    public async Task<IReadOnlyCollection<SteamSuggestItem>> GetSteamSuggests(string steamAppName)
    {
        var steamSuggestsHtml = await _storeSteamPoweredClient.GetSuggests(steamAppName);

        return steamSuggestsHtml.Split("ds_collapse_flag")
            .Where(s => s.Contains(NameHtmlTag) && s.Contains(PriceHtmlTag))
            .Select(s => new SteamSuggestItem(s))
            .ToList();
    }

    public async Task AddOrUpdateSteamApplications(List<SteamSuggestItem> steamSuggestItems)
    {
        if (steamSuggestItems.Count == default)
            return;

        var steamSuggestIds = steamSuggestItems.Select(item => item.AppId).ToList();
        var existingApps = await _steamAppRepository.GetSteamsApplicationsByIds(steamSuggestIds);
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
        await _steamAppRepository.AddRange(newEntities);

        return newEntities;
    }

    private async Task UpdateSteamApplications(List<SteamSuggestItem> steamSuggestItems, List<SteamAppEntity> existingSteamApps)
    {
        if (existingSteamApps.Count == default)
            return;

        var appsWithDifferentPrice = existingSteamApps
            .Where(entity => steamSuggestItems.Any(item => item.AppId == entity.SteamAppId 
                                                           && item.Price < entity.Price))
            .ToList();

        await _steamAppRepository.UpdateRange(
            source: steamSuggestItems, 
            entities: existingSteamApps, 
            find: (src, target) => target.First(t => t.SteamAppId == src.AppId));

        await NotifyUsers(steamSuggestItems, appsWithDifferentPrice);
    }

    private async Task NotifyUsers(IReadOnlyCollection<SteamSuggestItem> steamSuggestItems, IEnumerable<SteamAppEntity> appsWithDifferentPrice)
    {
        var trackedUsers = appsWithDifferentPrice.SelectMany(x => x.TrackedUsers).ToList();
        if (trackedUsers.Count == default)
            return;

        var usersWithDiscountedApps = GetUsersWithDiscountedApps(steamSuggestItems, trackedUsers);

        await _telegramNotificationService.NotifyUsersOfPriceDrop(usersWithDiscountedApps);
    }

    private static Dictionary<long, List<SteamSuggestItem>> GetUsersWithDiscountedApps(
        IReadOnlyCollection<SteamSuggestItem> steamSuggestItems,
        List<UserAppTrackingEntity> trackedUsers)
    {
        var usersWithDiscountedApps = new Dictionary<long, List<SteamSuggestItem>>();
        foreach (var trackedUser in trackedUsers)
        {
            if (usersWithDiscountedApps.TryGetValue(trackedUser.User.TelegramChatId, out var value))
            {
                value.Add(steamSuggestItems.First(c => c.AppId == trackedUser.SteamApp.SteamAppId));
            }
            else
            {
                usersWithDiscountedApps.Add(trackedUser.User.TelegramChatId, new List<SteamSuggestItem> { steamSuggestItems.First(x => trackedUser.SteamApp.SteamAppId == x.AppId) });
            }
        }

        return usersWithDiscountedApps;
    }
}