using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

/// <summary>
/// Callback for sent main menu
/// </summary>
internal sealed class MainMenuCallback : BaseCallback
{
    public MainMenuCallback(
        ITelegramBotClient botClient,
        IUserAppTrackingService userAppTrackingService,
        ITelegramNotificationService telegramNotificationService) : base(botClient, userAppTrackingService, telegramNotificationService)
    {
    }

    public override string Name => TelegramCallbacks.MainMenuCallback;

    public override Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        => TelegramNotificationService.SendStartInlineKeyBoard(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            cancellationToken: cancellationToken);
}