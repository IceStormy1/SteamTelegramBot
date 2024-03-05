using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

public interface IUserAppTrackingRepository : IBaseRepository<UserAppTrackingEntity>
{
    /// <summary>
    /// Returns a list of applications that the user is tracking
    /// </summary>
    /// <param name="telegramUserId">User id in Telegram</param>
    Task<List<SteamAppEntity>> GetTrackedApplicationsByTelegramId(long telegramUserId);

    /// <summary>
    /// Returns true if link already exists
    /// </summary>
    /// <param name="telegramUserId">User id in Telegram</param>
    /// <param name="steamAppId">Steam application id <see cref="SteamAppEntity.SteamAppId"/></param>
    Task<bool> HasTrackedApplication(long telegramUserId, int steamAppId);

    Task<UserAppTrackingEntity> GetUserAppTracking(long telegramUserId, int steamAppId);
}