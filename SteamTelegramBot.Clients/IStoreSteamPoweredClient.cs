using Refit;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Clients;

public interface IStoreSteamPoweredClient
{
    /// <summary>
    /// Returns suggest items from steam 
    /// </summary>
    /// <param name="term">The word to search for</param>
    /// <param name="f">Country code</param>
    /// <param name="cc"></param>
    /// <param name="l"></param>
    /// <param name="use_store_query"></param>
    /// <returns></returns>
    [Get("/search/suggest")]
    public Task<string> GetSuggests([Query] string term, [Query] string f = "games", [Query] CountryCode cc = CountryCode.Ru, [Query] string l = "russian", [Query] byte use_store_query = 1);
}