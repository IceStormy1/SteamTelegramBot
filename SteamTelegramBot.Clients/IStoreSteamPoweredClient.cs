using Refit;
using SteamTelegramBot.Clients.Models;

namespace SteamTelegramBot.Clients;

public interface IStoreSteamPoweredClient
{
    [Get("/search/suggest")]
    public Task<string> GetSuggests([Query] string term, [Query] string f = "games", [Query] CountryCode cc = CountryCode.Ru, [Query] string l = "russian", [Query] byte use_store_query = 1);
}