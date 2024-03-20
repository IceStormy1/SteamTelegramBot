using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

/// <summary>
/// Represents a repository interface for Steam applications.
/// </summary>
public interface ISteamAppRepository : IBaseRepository<SteamAppEntity>
{
    /// <summary>
    /// Retrieves Steam applications by their IDs.
    /// </summary>
    /// <param name="steamAppsIds">The collection of Steam application IDs.</param>
    /// <returns>A list of Steam applications.</returns>
    Task<List<SteamAppEntity>> GetSteamApplicationsByIds(ICollection<int> steamAppsIds);

    /// <summary>
    /// Retrieves a Steam application by ID.
    /// </summary>
    /// <param name="steamAppId">The ID of the Steam application.</param>
    /// <returns>The Steam application.</returns>
    Task<SteamAppEntity> GetSteamApplicationById(long steamAppId);

    /// <summary>
    /// Returns <see cref="SteamAppEntity.SteamAppId"/> that already exists by <paramref name="steamAppsIds"/>
    /// </summary>
    /// <param name="steamAppsIds">Steam applications ids</param>
    /// <returns></returns>
    Task<List<int>> CheckSteamApplicationsByIds(ICollection<int> steamAppsIds);
}