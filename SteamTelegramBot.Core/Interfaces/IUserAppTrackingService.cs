using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Abstractions.Models.Applications;

namespace SteamTelegramBot.Core.Interfaces;

/// <summary>
/// Defines the interface for managing user application tracking.
/// </summary>
public interface IUserAppTrackingService
{
    /// <summary>
    /// Links a user with a specific application.
    /// </summary>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="steamApplicationId">The Steam application ID.</param>
    /// <returns>A tuple indicating whether the operation was successful and an optional error message.</returns>
    Task<(bool IsSuccess, string ErrorMessage)> LinkUserAndApplication(long telegramUserId, long steamApplicationId);

    /// <summary>
    /// Removes the link between a user and a specific application.
    /// </summary>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="steamApplicationId">The Steam application ID.</param>
    /// <returns>A tuple indicating whether the operation was successful and an optional error message.</returns>
    Task<(bool IsSuccess, string ErrorMessage)> RemoveLinkBetweenUserAndApplication(long telegramUserId, long steamApplicationId);

    /// <summary>
    /// Retrieves a list of tracked applications for a specific user.
    /// </summary>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="limit">The maximum number of items to retrieve.</param>
    /// <param name="offset">The number of items to skip before beginning to return results.</param>
    /// <returns>A list response containing tracked application items.</returns>
    Task<ListResponseDto<TrackedAppItemDto>> GetUserTrackedApps(long telegramUserId, byte limit, int offset);

    /// <summary>
    /// Retrieves the IDs of tracked applications for users.
    /// </summary>
    /// <param name="limit">The maximum number of items to retrieve.</param>
    /// <param name="offset">The number of items to skip before beginning to return results.</param>
    /// <returns>A list of tracked application IDs.</returns>
    Task<List<int>> GetUsersTrackedAppsIds(short limit, int offset);
}