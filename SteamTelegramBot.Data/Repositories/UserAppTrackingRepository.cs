using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class UserAppTrackingRepository : BaseRepository<UserAppTrackingEntity>, IUserAppTrackingRepository
{
    public UserAppTrackingRepository(SteamTelegramBotDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public Task<List<SteamAppEntity>> GetTrackedApplicationsByTelegramId(long telegramUserId)
        => DbSet.AsNoTracking()
            .Where(x => x.User.TelegramId == telegramUserId)
            .OrderBy(x=>x.SteamApp.Title)
            .Select(x => x.SteamApp)
            .ToListAsync();

    public Task<List<int>> GetTrackedSteamAppIds(short limit, int offset)
        => DbSet.AsNoTracking()
            .GroupBy(x => x.SteamAppId)
            .OrderBy(x => x.Key)
            .Skip(offset)
            .Take(limit)
            .Select(x => x.First().SteamApp.SteamAppId)
            .ToListAsync();

    public Task<bool> HasTrackedApplication(long telegramUserId, long steamApplicationId)
        => DbSet.AnyAsync(x => x.SteamApp.SteamAppId == steamApplicationId 
                               && x.User.TelegramId == telegramUserId);

    public Task<UserAppTrackingEntity> GetUserAppTracking(long telegramUserId, long steamAppId)
        => DbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SteamAppId == steamAppId
                                      && x.User.TelegramId == telegramUserId);
}