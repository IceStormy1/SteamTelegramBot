namespace SteamTelegramBot.Abstractions;

/// <summary>
/// Represents an entity that has a date and time of creation.
/// </summary>
public interface IHasCreatedAt
{
    /// <summary>
    /// Date and time the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}