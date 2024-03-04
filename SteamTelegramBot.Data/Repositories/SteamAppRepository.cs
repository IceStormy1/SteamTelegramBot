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

    public Task<List<SteamAppEntity>> GetSteamApplicationsByIds(IReadOnlyCollection<int> steamAppsIds)
        => DbContext.SteamApps
            .Include(x => x.TrackedUsers)
                .ThenInclude(x => x.User)
            .Include(x=>x.PriceHistory.Take(MaxPriceHistory))
            .Where(x => steamAppsIds.Contains(x.SteamAppId))
            .ToListAsync();
}