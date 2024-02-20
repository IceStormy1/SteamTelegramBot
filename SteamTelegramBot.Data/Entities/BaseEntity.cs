namespace SteamTelegramBot.Data.Entities;

public abstract class BaseEntity
{
    /// <summary>
    /// Entity identifier
    /// </summary>
    public long Id { get; set; }
}