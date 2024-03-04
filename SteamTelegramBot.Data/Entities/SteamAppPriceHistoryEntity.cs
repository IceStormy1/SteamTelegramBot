using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Data.Entities;

public sealed class SteamAppPriceHistoryEntity : BaseEntity, IHasCreatedAt
{
    public long SteamAppId { get; set; }
    /// <inheritdoc cref="SteamAppEntity"/>
    public SteamAppEntity SteamApp { get; set; }

    /// <summary>
    /// Price of the app
    /// </summary>
    /// <remarks>
    /// null - Free app;
    /// 0 - The game has become free
    /// </remarks>
    public decimal? Price { get; set; }

    public PriceType PriceType { get; set; }

    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<TelegramNotificationEntity> TelegramNotifications { get; set; } = new();
}