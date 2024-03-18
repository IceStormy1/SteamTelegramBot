using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Abstractions.Models.Applications;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Extensions;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Core.Services;

internal sealed class UserAppTrackingService : BaseService, IUserAppTrackingService
{
    private readonly IUserAppTrackingRepository _userAppTrackingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISteamAppRepository _steamAppRepository;

    public UserAppTrackingService(
        IMapper mapper, 
        ILogger<UserAppTrackingService> logger, 
        IUserAppTrackingRepository userAppTrackingRepository,
        IUserRepository userRepository,
        ISteamAppRepository steamAppRepository) : base(mapper, logger)
    {
        _userAppTrackingRepository = userAppTrackingRepository;
        _userRepository = userRepository;
        _steamAppRepository = steamAppRepository;
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> LinkUserAndApplication(long telegramUserId, long steamApplicationId)
    {
        var hasLink = await _userAppTrackingRepository.HasTrackedApplication(telegramUserId, steamApplicationId);

        if (hasLink)
            return (IsSuccess: false, ErrorMessage: "Вы уже отслеживаете данную игру");

        var userEntity = await _userRepository.GetUserByTelegramId(telegramUserId);
        if (userEntity is null)
            return (IsSuccess: false, ErrorMessage: "Пользователь не найден");

        var steamAppEntity = await _steamAppRepository.GetSteamApplicationById(steamApplicationId);
        if (steamAppEntity is null)
            return (IsSuccess: false, ErrorMessage: "Игра не найдена");

        var entity = new UserAppTrackingEntity { SteamAppId = steamAppEntity.Id, UserId = userEntity.Id };
        await _userAppTrackingRepository.Add(entity);

        return (IsSuccess: true, ErrorMessage: null);
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> RemoveLinkBetweenUserAndApplication(long telegramUserId, long steamApplicationId)
    {
        var link = await _userAppTrackingRepository.GetUserAppTracking(telegramUserId, steamApplicationId);

        if (link is null)
            return (IsSuccess: false, "В вашем списке нет такой игры");

        await _userAppTrackingRepository.Remove(link);

        return (IsSuccess: true, ErrorMessage: null);
    }

    public async Task<ListResponseDto<TrackedAppItemDto>> GetUserTrackedApps(long telegramUserId, byte limit, int offset)
    {
        var trackedApps = await _userAppTrackingRepository.GetTrackedApplicationsByTelegramId(telegramUserId, limit, offset);

        return new ListResponseDto<TrackedAppItemDto>
        {
            Total = trackedApps.Total,
            Items = trackedApps.Items.Select((app, index) => app.ToTrackedAppItem(offset + index)).ToList()
        };
    }

    public Task<List<int>> GetUsersTrackedAppsIds(short limit, int offset)
        => _userAppTrackingRepository.GetTrackedSteamAppIds(limit, offset);
}