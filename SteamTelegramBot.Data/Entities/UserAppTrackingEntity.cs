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


    public long SteamAppId { get; set; }
    /// <inheritdoc cref="SteamAppEntity"/>
    public SteamAppEntity SteamApp { get; set; }

    public DateTime CreatedAt { get; set; }
}