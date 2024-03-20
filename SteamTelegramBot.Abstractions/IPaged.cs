namespace SteamTelegramBot.Abstractions;

/// <summary>
/// Represents a pageable interface with properties for pagination
/// </summary>
public interface IPaged
{
    /// <summary>
    /// The default page number
    /// </summary>
    public const short DefaultPage = 1;

    /// <summary>
    /// Value indicating whether to ignore pagination
    /// </summary>
    public bool? Ignore { get; set; }

    /// <summary>
    /// Size of each page
    /// </summary>
    public byte Size { get; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// Total number of items
    /// </summary>
    public int Total { get; set; }
}