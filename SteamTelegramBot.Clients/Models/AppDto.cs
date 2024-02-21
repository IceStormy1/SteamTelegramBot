using Newtonsoft.Json;
using SteamTelegramBot.Clients.Converters;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Clients.Models;

[JsonConverter(typeof(ResultDataConverter))]
public sealed class ResultData
{
    public App Item { get; set; }
}

public sealed class App
{
    public bool Success { get; set; }
    public AppDetail Data { get; set; }
}

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

    public Platforms Platforms { get; set; }

    [JsonProperty("release_date")]
    public ReleaseDate ReleaseDate { get; set; }
}

public sealed class Fullgame
{
    public int AppID { get; set; }
    public string Name { get; set; }
}

public sealed class PriceOverview
{
    public string Currency { get; set; }
    [JsonProperty("final_formatted")]
    public string FinalFormatted { get; set; }
    public decimal Initial { get; set; }
    public decimal Final { get; set; }
    public int DiscountPercent { get; set; }
    public int[] Packages { get; set; }
}

public sealed class Platforms
{
    public bool Windows { get; set; }
    public bool Mac { get; set; }
    public bool Linux { get; set; }
}

public sealed class ReleaseDate
{
    public bool ComingSoon { get; set; }
    public string Date { get; set; }
}