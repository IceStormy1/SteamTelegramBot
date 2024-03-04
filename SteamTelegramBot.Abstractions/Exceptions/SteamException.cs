namespace SteamTelegramBot.Abstractions.Exceptions;

public sealed class SteamException : Exception
{
    public SteamException(
        Exception innerException,
        string message) : base(message, innerException)
    {

    }

    public SteamException(string message, long steamAppId) : base(message)
    {
        SteamAppId = steamAppId;
    }

    public SteamException(string message) : base(message)
    {
        
    }

    public long? SteamAppId { get; init; }
}