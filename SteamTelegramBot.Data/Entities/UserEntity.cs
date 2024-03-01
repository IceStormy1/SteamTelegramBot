using SteamTelegramBot.Abstractions;

namespace SteamTelegramBot.Data.Entities;

public sealed class UserEntity : BaseEntity, IHasCreatedAt, IHasUpdatedAt
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }

    public long TelegramId { get; set; }
    public long TelegramChatId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<UserAppTrackingEntity> TrackedApps { get; set; } = new();
}