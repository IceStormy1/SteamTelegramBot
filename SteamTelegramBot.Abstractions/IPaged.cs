namespace SteamTelegramBot.Abstractions;

public interface IPaged
{
    public const short DefaultPage = 1;

    public bool? Ignore { get; set; }

    public byte Size { get; }

    public int Current { get; set; }
    public int Total { get; set; }
}