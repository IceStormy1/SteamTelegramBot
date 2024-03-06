using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Callbacks;

internal sealed class RemoveAppCallback : BaseCallback
{
    public RemoveAppCallback(
        ITelegramBotClient botClient, 
        IUserAppTrackingService userAppTrackingService,
        ITelegramNotificationService telegramNotificationService) : base(botClient, userAppTrackingService, telegramNotificationService)
    {
    }

    public override string Name => TelegramConstants.RemoveAppCallback;

    public override async Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var trackedApps = await UserAppTrackingService.GetUserTrackedApps(callbackQuery.From.Id);

        await BotClient.EditMessageTextAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            text: TelegramConstants.RemoveGameCallbackMessage,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByAppAction(trackedApps, AppAction.Remove),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }
}