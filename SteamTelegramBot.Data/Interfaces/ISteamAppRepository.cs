using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

public interface ISteamAppRepository : IBaseRepository<SteamAppEntity>
{
    Task<List<SteamAppEntity>> GetSteamsApplicationsByIds(IReadOnlyCollection<int> steamAppsIds);
}