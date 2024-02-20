using Refit;
using SteamTelegramBot.Clients.Models;

namespace SteamTelegramBot.Clients;

public interface IStoreSteamApiClient
{
    /// <summary>
    /// Возвращает информацию по залогиненному пользователю
    /// </summary>
    /// <returns></returns>
    [Get("/appdetails")]
    public Task<ResultData> GetAppDetails([Query] int appids, [Query] CountryCode cc = CountryCode.Ru);
}