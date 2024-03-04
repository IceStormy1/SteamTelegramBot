namespace SteamTelegramBot.Data.Entities;

public sealed class TelegramNotificationEntity : BaseEntity
{
    public long UserAppTrackingId { get; set; }
    public UserAppTrackingEntity UserAppTracking { get; set; }

    public long SteamAppPriceId { get; set; }
    public SteamAppPriceHistoryEntity SteamAppPrice { get; set;}

    public bool WasSent { get; set; }
}