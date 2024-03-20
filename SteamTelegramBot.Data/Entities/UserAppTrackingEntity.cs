using SteamTelegramBot.Abstractions;

namespace SteamTelegramBot.Data.Entities;

/// <summary>
/// Applications that the user is tracking
/// </summary>
public sealed class UserAppTrackingEntity : BaseEntity, IHasCreatedAt
{
    public long UserId { get; set; }

    /// <inheritdoc cref="UserEntity"/>
    public UserEntity User { get; set; }

    /// <summary>
    /// Steam application id
    /// </summary>
    public long SteamAppId { get; set; }

    /// <inheritdoc cref="SteamAppEntity"/>
    public SteamAppEntity SteamApp { get; set; }

    /// <inheritdoc cref="IHasCreatedAt.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    public List<TelegramNotificationEntity> TelegramNotifications { get; set; } = new();
}