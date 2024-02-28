using Newtonsoft.Json;
using SteamTelegramBot.Clients.Converters;

namespace SteamTelegramBot.Clients.Models;

[JsonConverter(typeof(AppDetailsResultDataConverter))]
public sealed class AppDetailsResultData
{
    public App Item { get; set; }
}