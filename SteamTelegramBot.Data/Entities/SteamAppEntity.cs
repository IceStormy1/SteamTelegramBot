namespace SteamTelegramBot.Data.Entities;

public sealed class SteamAppEntity : BaseEntity
{
    public string Title { get; set; }
    public int SteamAppId { get; set; }
    public string HeaderImage { get; set; }

    public List<SteamAppPriceHistoryEntity> PriceHistory { get; set; } = new();

    public List<UserAppTrackingEntity> TrackedUsers { get; set; } = new();
}