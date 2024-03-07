﻿using SteamTelegramBot.Abstractions.Models;

namespace SteamTelegramBot.Core.Interfaces;

public interface IUserAppTrackingService
{
    Task<(bool IsSuccess, string ErrorMessage)> LinkUserAndApplication(long telegramUserId, int steamApplicationId);
    Task<(bool IsSuccess, string ErrorMessage)> RemoveLinkBetweenUserAndApplication(long telegramUserId, int steamApplicationId);
    Task<List<TrackedAppItemDto>> GetUserTrackedApps(long telegramUserId);
}