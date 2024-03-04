using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

public interface ISteamAppRepository : IBaseRepository<SteamAppEntity>
{
    Task<List<SteamAppEntity>> GetSteamApplicationsByIds(IReadOnlyCollection<int> steamAppsIds);
}