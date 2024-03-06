using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Callbacks;

internal sealed class AddAppCallback : BaseCallback
{
    public AddAppCallback(
        ITelegramBotClient botClient, 
        IUserAppTrackingService userAppTrackingService,
        ITelegramNotificationService telegramNotificationService) : base(botClient, userAppTrackingService, telegramNotificationService)
    {
       
    }

    public override string Name => TelegramConstants.AddAppCallback;

    public override Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        => BotClient.EditMessageTextAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            text: TelegramConstants.AddGameCallbackMessage,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.AddGame),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
}