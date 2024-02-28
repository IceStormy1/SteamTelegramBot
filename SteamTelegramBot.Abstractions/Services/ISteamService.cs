using SteamTelegramBot.Abstractions.Models;

namespace SteamTelegramBot.Abstractions.Services;

public interface ISteamService
{
    /// <summary>
    /// Returns all steam apps
    /// </summary>
    /// <remarks>
    /// Steam does not support pagination, so the Steam API returns all applications
    /// </remarks>
    Task<IReadOnlyCollection<AppItemDto>> GetAllSteamApps();

    /// <summary>
    /// Returns basic info for steam application
    /// </summary>
    /// <param name="steamAppName">Steam application id</param>
    Task<IReadOnlyCollection<SteamSuggestItem>> GetSteamSuggests(string steamAppName);
}