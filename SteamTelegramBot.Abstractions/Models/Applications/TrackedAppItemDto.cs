namespace SteamTelegramBot.Abstractions.Models.Applications;

/// <summary>
/// User tracked application
/// </summary>
public sealed class TrackedAppItemDto
{
    /// <summary>
    /// Index of list
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Application name
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Link to the page in the store
    /// </summary>
    public string Link { get; init; }

    /// <summary>
    /// Application id
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Formatted title application
    /// </summary>
    /// <example>
    /// 1. Half-life
    /// </example>
    public string FormattedTitle => $"{Index}. {Name}";
}