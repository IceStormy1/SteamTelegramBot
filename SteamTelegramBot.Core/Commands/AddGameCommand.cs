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
internal sealed class AddGameCommand : BaseCommand
{
    private readonly ISteamService _steamService;

    public AddGameCommand(
        ITelegramBotClient botClient, 
        ISteamService steamService) : base(botClient)
    {
        _steamService = steamService;
    }

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
                text: TelegramMessages.NeedToApplicationName,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
            return;
        }

        var steamApps = await _steamService.GetSteamSuggests(appName, filterByExistingApps: true);

        if (steamApps.Count == default)
        {
            await BotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: TelegramMessages.GameNotFound,
                cancellationToken: cancellationToken);
            return;
        }

        await BotClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Выберите один из вариантов, нажав на соответствующую кнопку",
            replyMarkup: InlineKeyBoardHelper.GetAddGameInlineKeyboard(steamApps),
            cancellationToken: cancellationToken
        );
    }
}