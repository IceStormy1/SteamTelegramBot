using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class TelegramNotificationRepository : BaseRepository<TelegramNotificationEntity>,
    ITelegramNotificationRepository
{
    public TelegramNotificationRepository(
        SteamTelegramBotDbContext dbContext,
        IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<Dictionary<long, List<TelegramNotificationEntity>>> GetUnNotifiedUsers(byte limit, int offset)
    {
        var usersWithUnsentNotifications = await DbContext.TelegramNotifications
            .Include(c => c.UserAppTracking)
            .Include(c => c.SteamAppPrice)
                .ThenInclude(x => x.SteamApp)
            .Where(x => !x.WasSent)
            .GroupBy(x => x.UserAppTracking.User.TelegramChatId)
            .Skip(offset)
            .Take(limit)
            .Select(x => new
            {
                UserId = x.Key,
                Notifications = x.Select(c => c)
            })
            .ToDictionaryAsync(
                x => x.UserId,
                x => x.Notifications.ToList());

        return usersWithUnsentNotifications;
    }
}