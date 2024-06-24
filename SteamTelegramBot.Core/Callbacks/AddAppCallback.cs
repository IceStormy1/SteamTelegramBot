using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Callbacks;

internal sealed class AddAppCallback(
    ITelegramBotClient botClient,
    IUserAppTrackingService userAppTrackingService,
    ITelegramNotificationService telegramNotificationService)
    : BaseCallback(botClient, userAppTrackingService, telegramNotificationService)
{
    public override string Name => TelegramCallbacks.AddAppCallback;

    public override Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        => BotClient.EditMessageTextAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            text: TelegramBotMessages.AddGameCallbackMessage,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.AddGame),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
}