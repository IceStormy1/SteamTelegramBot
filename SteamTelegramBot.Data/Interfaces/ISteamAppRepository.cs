using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

public interface ISteamAppRepository : IBaseRepository<SteamAppEntity>
{
    Task<List<SteamAppEntity>> GetSteamApplicationsByIds(ICollection<int> steamAppsIds);
    Task<SteamAppEntity> GetSteamApplicationById(int steamAppId);

    /// <summary>
    /// Returns <see cref="SteamAppEntity.SteamAppId"/> that already exists by <paramref name="steamAppsIds"/>
    /// </summary>
    /// <param name="steamAppsIds">Steam applications ids</param>
    /// <returns></returns>
    Task<List<int>> CheckSteamApplicationsByIds(ICollection<int> steamAppsIds);
}