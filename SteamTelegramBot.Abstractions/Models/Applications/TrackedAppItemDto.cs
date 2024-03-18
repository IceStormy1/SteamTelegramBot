namespace SteamTelegramBot.Abstractions.Models.Applications;

public sealed class TrackedAppItemDto
{
    /// <summary>
    /// Index of list
    /// </summary>
    public int Index { get; init; }
    public string Name { get; init; }
    public string Link { get; init; }
    public long Id { get; init; }

    public string FormattedTitle => $"{Index}. {Name}";
}