using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Abstractions.Models.Applications;

namespace SteamTelegramBot.Core.Interfaces;

public interface IUserAppTrackingService
{
    Task<(bool IsSuccess, string ErrorMessage)> LinkUserAndApplication(long telegramUserId, long steamApplicationId);
    Task<(bool IsSuccess, string ErrorMessage)> RemoveLinkBetweenUserAndApplication(long telegramUserId, long steamApplicationId);
    Task<ListResponseDto<TrackedAppItemDto>> GetUserTrackedApps(long telegramUserId, byte limit, int offset);
    Task<List<int>> GetUsersTrackedAppsIds(short limit, int offset);
}