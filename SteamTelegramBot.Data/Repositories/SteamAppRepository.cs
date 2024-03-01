using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class SteamAppRepository : BaseRepository<SteamAppEntity>, ISteamAppRepository
{
    public SteamAppRepository(SteamTelegramBotDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public Task<List<SteamAppEntity>> GetSteamsApplicationsByIds(IReadOnlyCollection<int> steamAppsIds)
        => DbContext.SteamApps
            .Include(x => x.TrackedUsers)
                .ThenInclude(x => x.User)
            .Where(x => steamAppsIds.Contains(x.SteamAppId))
            .ToListAsync();
}