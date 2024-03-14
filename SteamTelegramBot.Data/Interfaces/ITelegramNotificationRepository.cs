using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

public interface ITelegramNotificationRepository : IBaseRepository<TelegramNotificationEntity>
{
    /// <returns>
    /// Key - telegram chat id; Value - Unsent notifications
    /// </returns>
    Task<Dictionary<long, List<TelegramNotificationEntity>>> GetUnNotifiedUsers(List<int> applicationIds);
}