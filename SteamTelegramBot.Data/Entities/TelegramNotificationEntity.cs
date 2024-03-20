﻿namespace SteamTelegramBot.Data.Entities;

/// <summary>
/// Represents an entity for a Telegram notification.
/// </summary>
public sealed class TelegramNotificationEntity : BaseEntity
{
    /// <summary>
    /// ID of the user application tracking associated with the notification
    /// </summary>
    public long UserAppTrackingId { get; set; }

    /// <inheritdoc cref="UserAppTrackingEntity"/>
    public UserAppTrackingEntity UserAppTracking { get; set; }

    /// <summary>
    /// ID of the Steam application price associated with the notification.
    /// </summary>
    public long SteamAppPriceId { get; set; }

    /// <inheritdoc cref="SteamAppPriceHistoryEntity"/>
    public SteamAppPriceHistoryEntity SteamAppPrice { get; set;}

    /// <summary>
    /// Indicating whether the notification has been sent.
    /// </summary>
    public bool WasSent { get; set; }
}