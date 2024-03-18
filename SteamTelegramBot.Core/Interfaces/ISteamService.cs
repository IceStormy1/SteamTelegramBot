using SteamTelegramBot.Abstractions.Models.Applications;

namespace SteamTelegramBot.Core.Interfaces;

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
    /// <param name="steamAppName">Steam application name</param>
    /// <param name="filterByExistingApps">True - returns only those applications that are in the database</param>
    Task<IReadOnlyCollection<SteamSuggestItem>> GetSteamSuggests(string steamAppName, bool filterByExistingApps = false);

    /// <summary>
    /// Updates the existing app if the price is different
    /// </summary>
    /// <param name="steamSuggestItems">Items from steam suggest</param>
    Task AddOrUpdateSteamApplications(List<SteamSuggestItem> steamSuggestItems);
}