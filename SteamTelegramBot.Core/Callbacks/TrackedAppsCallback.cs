using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

internal sealed class TrackedAppsCallback : BaseCallback
{
    public TrackedAppsCallback(
        ITelegramBotClient botClient,
        IUserAppTrackingService userAppTrackingService,
        ITelegramNotificationService telegramNotificationService) : base(botClient, userAppTrackingService,
        telegramNotificationService)
    {
    }

    public override string Name => TelegramConstants.TrackedAppsCallback;

    public override Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        => TelegramNotificationService.SendTrackedApps(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            telegramUserId: callbackQuery.From.Id,
            cancellationToken: cancellationToken);
}