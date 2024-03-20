namespace SteamTelegramBot.Abstractions.Models.Applications;

/// <summary>
/// Steam application
/// </summary>
public sealed class AppItemDto
{
    /// <summary>
    /// Application Id
    /// </summary>
    public int AppId { get; init; }

    /// <summary>
    /// Application Name
    /// </summary>
    public string Name { get; init; }
}