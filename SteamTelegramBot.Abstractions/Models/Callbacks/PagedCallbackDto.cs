using Newtonsoft.Json;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public class PagedCallbackDto : BaseCallbackDto, IPaged
{
    [JsonIgnore]
    public byte Size => 5;

    public bool? Ignore { get; set; }

    /// <summary>
    /// Current page
    /// </summary>
    public int Current { get; set; } = 1;

    /// <summary>
    /// Total pages
    /// </summary>
    public int Total { get; set; }
}