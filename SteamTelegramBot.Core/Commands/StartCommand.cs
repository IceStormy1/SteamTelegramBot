using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Commands;

/// <summary>
/// Represents a command for starting interaction with a Telegram bot.
/// </summary>
internal sealed class StartCommand(
    ITelegramBotClient botClient,
    ITelegramNotificationService telegramNotificationService)
    : BaseCommand(botClient)
{
    public override string Name => TelegramCommands.StartCommand;

    public override async Task Execute(Message message, CancellationToken cancellationToken)
    {
        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramBotMessages.StartMessage,
            cancellationToken: cancellationToken);

        await telegramNotificationService.SendStartInlineKeyBoard(chatId: message.Chat.Id, cancellationToken);
    }
}