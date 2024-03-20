namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public class BaseCallbackDto
{
    /// <summary>
    /// Callback name
    /// </summary>
    public virtual string Name { get; init; }
}