using Refit;
using SteamTelegramBot.Clients.Models;

namespace SteamTelegramBot.Clients;

public interface IStoreSteamApiClient
{
    /// <summary>
    /// Returns app details by <paramref name="appids"/>
    /// </summary>
    [Get("/appdetails")]
    public Task<AppDetailsResultData> GetAppDetails([Query] int appids, [Query] CountryCode cc = CountryCode.Ru);
}