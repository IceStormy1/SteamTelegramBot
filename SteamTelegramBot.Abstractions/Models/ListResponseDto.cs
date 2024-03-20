namespace SteamTelegramBot.Abstractions.Models;

public sealed class ListResponseDto<TItem>
{
    /// <summary>
    /// Total items
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    public List<TItem> Items { get; set; } = new();

    /// <summary>
    /// Returns <see cref="ListResponseDto{TItem}"/> with empty <see cref="ListResponseDto{TItem}.Items"/> and default <see cref="ListResponseDto{TItem}.Total"/>
    /// </summary>
    public static ListResponseDto<TItem> Empty => new();
}