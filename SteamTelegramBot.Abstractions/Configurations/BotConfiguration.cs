namespace SteamTelegramBot.Abstractions.Configurations;

public sealed class BotConfiguration
{
    /// <summary>
    /// Token from BotFather
    /// </summary>
    public string BotToken { get; init; } = default!;

    /// <summary>
    /// Hosted Addres
    /// </summary>
    /// <example>
    /// https://ngrok*
    /// </example>
    public string HostAddress { get; init; } = default!;

    /// <summary>
    /// Route for accessing the bot
    /// </summary>
    public string Route { get; init; } = default!;

    /// <summary>
    /// Secret for hook
    /// </summary>
    public string SecretToken { get; init; } = default!;
}