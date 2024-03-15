using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class SteamAppRepository : BaseRepository<SteamAppEntity>, ISteamAppRepository
{
    private const byte MaxPriceHistory = 2;

    public SteamAppRepository(
        SteamTelegramBotDbContext dbContext,
        IMapper mapper) : base(dbContext, mapper)
    {
    }

    public Task<List<SteamAppEntity>> GetSteamApplicationsByIds(ICollection<int> steamAppsIds)
        => DbSet
            .Include(x => x.TrackedUsers)
            .ThenInclude(x => x.User)
            .Include(x => x.PriceHistory.OrderByDescending(x=>x.Version).Take(MaxPriceHistory))
            .Where(x => steamAppsIds.Contains(x.SteamAppId))
            .ToListAsync();

    public Task<SteamAppEntity> GetSteamApplicationById(long steamAppId)
        => DbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SteamAppId == steamAppId);

    public Task<List<int>> CheckSteamApplicationsByIds(ICollection<int> steamAppsIds)
        => DbSet.Where(x => steamAppsIds.Contains(x.SteamAppId))
            .Select(x => x.SteamAppId)
            .ToListAsync();
}