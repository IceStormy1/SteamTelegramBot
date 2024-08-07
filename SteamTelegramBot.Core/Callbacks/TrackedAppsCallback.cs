﻿using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

/// <summary>
/// Callback when the user chooses to get a list of tracked applications
/// </summary>
internal sealed class TrackedAppsCallback(
    ITelegramBotClient botClient,
    IUserAppTrackingService userAppTrackingService,
    ITelegramNotificationService telegramNotificationService)
    : BaseCallback(botClient, userAppTrackingService,
        telegramNotificationService)
{
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