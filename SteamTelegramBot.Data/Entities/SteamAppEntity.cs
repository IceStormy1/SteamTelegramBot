using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Data.Entities;

public sealed class SteamAppEntity : BaseEntity
{
    public string Title { get; set; }
    public AppType Type { get; set; }
    public long SteamAppId { get; set; }
    public string ShortDescription { get; set; }
    public string HeaderImage { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal Price { get; set; }

    public List<UserAppTrackingEntity> TrackedUsers { get; set; }
}