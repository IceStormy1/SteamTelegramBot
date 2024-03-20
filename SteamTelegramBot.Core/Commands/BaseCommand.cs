using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Commands;

internal abstract class BaseCommand
{
    protected readonly ITelegramBotClient BotClient;

    protected BaseCommand(ITelegramBotClient botClient)
    {
        BotClient = botClient;
    }

    public abstract string Name { get; }

    public abstract Task Execute(Message message, CancellationToken cancellationToken);
}