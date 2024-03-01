using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Data.Entities;

public sealed class SteamAppEntity : BaseEntity
{
    public string Title { get; set; }
    public int SteamAppId { get; set; }
    public string HeaderImage { get; set; }

    /// <summary>
    /// Price of the app
    /// </summary>
    /// <remarks>
    /// null - Free app;
    /// 0 - The game has become free
    /// </remarks>
    public decimal? Price { get; set; }

    public PriceType PriceType { get; set; }

    public List<UserAppTrackingEntity> TrackedUsers { get; set; } = new();
}