using Newtonsoft.Json;

namespace SteamTelegramBot.Clients.Models;

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