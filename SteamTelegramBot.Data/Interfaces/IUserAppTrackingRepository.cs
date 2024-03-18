﻿using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

public interface IUserAppTrackingRepository : IBaseRepository<UserAppTrackingEntity>
{
    /// <summary>
    /// Returns a list of applications that the user is tracking
    /// </summary>
    /// <param name="telegramUserId">User id in Telegram</param>
    /// <param name="limit">Limit of applications</param>
    /// <param name="offset">Offset</param>
    Task<ListResponseDto<SteamAppEntity>> GetTrackedApplicationsByTelegramId(long telegramUserId, byte limit, int offset);

    Task<List<int>> GetTrackedSteamAppIds(short limit, int offset);

    /// <summary>
    /// Returns true if link already exists
    /// </summary>
    /// <param name="telegramUserId">User id in Telegram</param>
    /// <param name="steamApplicationId">Steam application id <see cref="SteamAppEntity.SteamAppId"/></param>
    Task<bool> HasTrackedApplication(long telegramUserId, long steamApplicationId);

    Task<UserAppTrackingEntity> GetUserAppTracking(long telegramUserId, long steamAppId);
}