using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

/// <summary>
/// Represents a repository interface for user application tracking.
/// </summary>
public interface IUserAppTrackingRepository : IBaseRepository<UserAppTrackingEntity>
{
    /// <summary>
    /// Returns a list of applications that the user is tracking
    /// </summary>
    /// <param name="telegramUserId">User id in Telegram</param>
    /// <param name="limit">Limit of applications</param>
    /// <param name="offset">Offset</param>
    Task<ListResponseDto<SteamAppEntity>> GetTrackedApplicationsByTelegramId(long telegramUserId, byte limit, int offset);

    /// <summary>
    /// Retrieves a list of tracked Steam application IDs.
    /// </summary>
    /// <param name="limit">The limit of application IDs to retrieve.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>A list of tracked Steam application IDs.</returns>
    Task<List<int>> GetTrackedSteamAppIds(short limit, int offset);

    /// <summary>
    /// Returns true if link already exists
    /// </summary>
    /// <param name="telegramUserId">User id in Telegram</param>
    /// <param name="steamApplicationId">Steam application id <see cref="SteamAppEntity.SteamAppId"/></param>
    Task<bool> HasTrackedApplication(long telegramUserId, long steamApplicationId);

    /// <summary>
    /// Retrieves the user application tracking entity by the specified Telegram user ID and Steam application ID.
    /// </summary>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="steamAppId">The Steam application ID.</param>
    /// <returns>The user application tracking entity.</returns>
    Task<UserAppTrackingEntity> GetUserAppTracking(long telegramUserId, long steamAppId);
}