using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class TelegramNotificationRepository(
    SteamTelegramBotDbContext dbContext,
    IMapper mapper) : BaseRepository<TelegramNotificationEntity>(dbContext, mapper), ITelegramNotificationRepository
{
    public async Task<Dictionary<long, List<TelegramNotificationEntity>>> GetUnNotifiedUsers(List<int> applicationIds)
    {
        var usersWithUnsentNotifications = await DbContext.TelegramNotifications
            .Include(c => c.UserAppTracking)
            .Include(c => c.SteamAppPrice)
                .ThenInclude(x => x.SteamApp)
            .Where(x => !x.WasSent && applicationIds.Contains(x.SteamAppPrice.SteamApp.SteamAppId))
            .GroupBy(x => x.UserAppTracking.User.TelegramChatId)
            .Select(x => new
            {
                TelegramChatId = x.Key,
                Notifications = x.Select(c => c)
            })
            .ToDictionaryAsync(
                x => x.TelegramChatId,
                x => x.Notifications.ToList());

        return usersWithUnsentNotifications;
    }
}