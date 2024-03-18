namespace SteamTelegramBot.Abstractions.Models;

public sealed class ListResponseDto<TItem>
{
    public long Total { get; init; }

    public List<TItem> Items { get; init; }

    public static ListResponseDto<TItem> Empty => new();
}