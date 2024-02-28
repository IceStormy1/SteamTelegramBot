using Newtonsoft.Json;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Clients.Models;

public sealed class AppDetail
{
    public AppType Type { get; set; }
    public string Name { get; set; }

    [JsonProperty("steam_appid")]
    public int SteamAppId { get; set; }

    [JsonProperty("short_description")]
    public string ShortDescription { get; set; }

    public Fullgame FullGame { get; set; } // Optional

    [JsonProperty("header_image")]
    public string HeaderImage { get; set; }

    [JsonProperty("price_overview")]
    public PriceOverview PriceOverview { get; set; } // Optional

    [JsonProperty("release_date")]
    public ReleaseDate ReleaseDate { get; set; }
}