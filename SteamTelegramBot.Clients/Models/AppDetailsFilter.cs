using Newtonsoft.Json;

namespace SteamTelegramBot.Clients.Models;

/// <summary>
/// See <a href="https://wiki.teamfortress.com/wiki/User:RJackson/StorefrontAPI">appdetails block</a>
/// </summary>
public sealed class AppDetailsFilter
{
    public AppType Type { get; set; }
    public string Name { get; set; }

    [JsonProperty("steam_appid")]
    public string SteamAppId { get; set; }
}