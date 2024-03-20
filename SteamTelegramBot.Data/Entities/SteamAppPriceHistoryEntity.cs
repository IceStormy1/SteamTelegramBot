using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Data.Entities;

/// <summary>
/// Entity storing price history information for a Steam application.
/// </summary>
public sealed class SteamAppPriceHistoryEntity : BaseEntity, IHasCreatedAt
{
    public long SteamAppId { get; set; }
    /// <inheritdoc cref="SteamAppEntity"/>
    public SteamAppEntity SteamApp { get; set; }

    /// <summary>
    /// Price of the application
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Type of price
    /// </summary>
    public PriceType PriceType { get; set; }

    /// <summary>
    /// Version of the price history.
    /// </summary>
    public int Version { get; set; }

    /// <inheritdoc cref="IHasCreatedAt.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    public List<TelegramNotificationEntity> TelegramNotifications { get; set; } = new();
}