using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Abstractions.Services;
using SteamTelegramBot.Clients;

namespace SteamTelegramBot.Core.Services;

public sealed class SteamService : BaseService, ISteamService
{
    private const string NameHtmlTag = "match_name";
    private const string PriceHtmlTag = "match_subtitle";

    private readonly ISteamWebApiClient _steamWebApiClient;
    private readonly IStoreSteamPoweredClient _storeSteamPoweredClient;

    public SteamService(
        IMapper mapper, 
        ILogger<BaseService> logger,
        ISteamWebApiClient steamWebApiClient,
        IStoreSteamPoweredClient storeSteamPoweredClient) : base(mapper, logger)
    {
        _steamWebApiClient = steamWebApiClient;
        _storeSteamPoweredClient = storeSteamPoweredClient;
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
}