namespace SteamTelegramBot.Data.Entities;

/// <summary>
/// Entity for a Steam application.
/// </summary>
public sealed class SteamAppEntity : BaseEntity
{
    /// <summary>
    /// Application title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Id from steam store
    /// </summary>
    public int SteamAppId { get; set; }

    /// <summary>
    /// Link to the app image in the store
    /// </summary>
    public string HeaderImage { get; set; }

    public List<SteamAppPriceHistoryEntity> PriceHistory { get; set; } = new();

    public List<UserAppTrackingEntity> TrackedUsers { get; set; } = new();
}