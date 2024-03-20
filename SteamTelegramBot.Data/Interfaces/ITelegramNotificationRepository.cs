using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

/// <summary>
/// Represents a repository interface for Telegram notifications.
/// </summary>
public interface ITelegramNotificationRepository : IBaseRepository<TelegramNotificationEntity>
{
    /// <summary>
    /// Retrieves unsent notifications for users based on the specified application IDs.
    /// </summary>
    /// <param name="applicationIds">The list of application IDs.</param>
    /// <returns>A dictionary where the key is the Telegram chat ID and the value is the list of unsent notifications.</returns>
    Task<Dictionary<long, List<TelegramNotificationEntity>>> GetUnNotifiedUsers(List<int> applicationIds);
}