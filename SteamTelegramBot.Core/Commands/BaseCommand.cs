using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Commands;

/// <summary>
/// Represents a base class for commands in a Telegram bot.
/// </summary>
internal abstract class BaseCommand
{
    protected readonly ITelegramBotClient BotClient;

    protected BaseCommand(ITelegramBotClient botClient)
    {
        BotClient = botClient;
    }

    /// <summary>
    /// Name of the command.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Executes the command based on the received message.
    /// </summary>
    /// <param name="message">The message received from the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public abstract Task Execute(Message message, CancellationToken cancellationToken);
}