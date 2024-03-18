using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class UserAppTrackingRepository : BaseRepository<UserAppTrackingEntity>, IUserAppTrackingRepository
{
    public UserAppTrackingRepository(SteamTelegramBotDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<ListResponseDto<SteamAppEntity>> GetTrackedApplicationsByTelegramId(long telegramUserId, byte limit,
        int offset)
    {
        var query = DbSet.AsNoTracking()
                .Where(x => x.User.TelegramId == telegramUserId);

        var totalApps = await query.LongCountAsync();

        if (totalApps == default)
            return ListResponseDto<SteamAppEntity>.Empty;

        var apps = await query
            .OrderBy(x => x.SteamApp.Title)
            .Skip(offset)
            .Take(limit)
            .Select(x => x.SteamApp)
            .ToListAsync();

        return new ListResponseDto<SteamAppEntity> { Items = apps, Total = totalApps };
    }

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