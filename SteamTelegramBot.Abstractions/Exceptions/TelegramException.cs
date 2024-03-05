namespace SteamTelegramBot.Abstractions.Exceptions;

public sealed class TelegramException : Exception
{
    public TelegramException(
        Exception innerException, 
        long chatId, 
        string message
        ) : base(message, innerException)
    {
        ChatId = chatId;
    }

    public TelegramException(long chatId, string message) : base(message)
    {
        ChatId = chatId;
    }

    public long ChatId { get; }
}