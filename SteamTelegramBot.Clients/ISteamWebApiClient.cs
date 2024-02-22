using Refit;
using SteamTelegramBot.Clients.Models;

namespace SteamTelegramBot.Clients;

public interface ISteamWebApiClient
{
    /// <summary>
    /// Returns all steam apps
    /// </summary>
    /// <remarks>
    /// There is no paging in this method
    /// </remarks>
    [Get("/ISteamApps/GetAppList/v2/")]
    public Task<IApiResponse<AppListResultData>> GetAllApps();
}