using SteamTelegramBot.Abstractions.Models;

namespace SteamTelegramBot.Core.Interfaces;

public interface IUserAppTrackingService
{
    Task<(bool IsSuccess, string ErrorMessage)> LinkUserAndApplication(long telegramUserId, long steamApplicationId);
    Task<(bool IsSuccess, string ErrorMessage)> RemoveLinkBetweenUserAndApplication(long telegramUserId, long steamApplicationId);
    Task<List<TrackedAppItemDto>> GetAllUserTrackedApps(long telegramUserId);
    Task<List<int>> GetUsersTrackedAppsIds(short limit, int offset);
}