namespace SteamTelegramBot.Abstractions.Models;

public sealed class ListResponseDto<TItem>
{
    public int Total { get; set; }

    public List<TItem> Items { get; set; } = new();

    public static ListResponseDto<TItem> Empty => new();
}