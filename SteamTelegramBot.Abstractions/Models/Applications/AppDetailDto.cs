namespace SteamTelegramBot.Abstractions.Models.Applications;

public sealed class AppDetailDto
{
    public string Name { get; init; }
    public int SteamAppId { get; init; }
    public string ShortDescription { get; init; }
    public string HeaderImage { get; init; }
    public decimal Price { get; init; }
    public DateOnly? ReleaseDate { get; init; }
}