namespace SteamTelegramBot.Abstractions.Exceptions;

public sealed class SteamException : Exception
{
    public SteamException(
        Exception innerException,
        string message) : base(message, innerException)
    {

    }

    public SteamException(string message, int steamAppId) : base(message)
    {
        SteamAppId = steamAppId;
    }

    public int? SteamAppId { get; init; }
}