using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
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

    public override string Name => TelegramCallbacks.TrackedAppsCallback;

    public override async Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var callbackData = GetCallbackData<TrackedAppsCallbackDto>(callbackQuery);

        await TelegramNotificationService.SendTrackedApps(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            telegramUserId: callbackQuery.From.Id,
            pageInfo: callbackData,
            action: AppAction.Get,
            cancellationToken: cancellationToken);
    }
}