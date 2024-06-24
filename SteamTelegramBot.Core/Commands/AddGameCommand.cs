using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Commands;

/// <summary>
/// Represents a command for adding a game
/// </summary>
internal sealed class AddGameCommand(
    ITelegramBotClient botClient,
    ISteamService steamService)
    : BaseCommand(botClient)
{
    public override string Name => TelegramCommands.AddGameCommand;

    public override async Task Execute(Message message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
            return;

        var splitMessage = message.Text.Split(' ');
        var appName = string.Join(" ", splitMessage.Skip(1));

        if (string.IsNullOrWhiteSpace(appName))
        {
            await BotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramBotMessages.NeedToApplicationName,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
            return;
        }

        var steamApps = await steamService.GetSteamSuggests(appName, filterByExistingApps: true);

        if (steamApps.Count == default)
        {
            await BotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramBotMessages.GameNotFound,
                cancellationToken: cancellationToken);
            return;
        }

        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramBotMessages.OptionsPrompt,
            replyMarkup: InlineKeyBoardHelper.GetAddGameInlineKeyboard(steamApps),
            cancellationToken: cancellationToken
        );
    }
}