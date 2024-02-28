using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Abstractions.Models;

public sealed class AppDetailDto
{
    public AppType Type { get; init; }
    public string Name { get; init; }
    public int SteamAppId { get; init; }
    public string ShortDescription { get; init; }
    public int FullGameId { get; init; }
    public string HeaderImage { get; init; }
    public decimal Price { get; init; }
    public DateOnly? ReleaseDate { get; init; }
}