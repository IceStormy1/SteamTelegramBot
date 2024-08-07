﻿using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

/// <summary>
/// Callback when user chosen application (remove or add)
/// </summary>
internal class ChosenAppCallback(
    ITelegramBotClient botClient,
    IUserAppTrackingService userAppTrackingService,
    ITelegramNotificationService telegramNotificationService)
    : BaseCallback(botClient, userAppTrackingService, telegramNotificationService)
{
    public override string Name => TelegramCallbacks.ChosenAppCallback;

    public override async Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chosenAppDto = GetCallbackData<ChosenAppCallbackDto>(callbackQuery);
        await OnChosenAppCallback(callbackQuery, chosenAppDto, cancellationToken);
    }

    private async Task OnChosenAppCallback(CallbackQuery callbackQuery, ChosenAppCallbackDto chosenAppDto, CancellationToken cancellationToken)
    {
        switch (chosenAppDto.Action)
        {
            case AppAction.Add:
                await OnChosenAddAppCallback(callbackQuery, chosenAppDto, cancellationToken);
                break;

            case AppAction.Remove:
                await OnChosenRemoveAppCallback(callbackQuery, chosenAppDto, cancellationToken);
                break;
        }
    }

    private async Task OnChosenAddAppCallback(CallbackQuery callbackQuery, ChosenAppCallbackDto chosenAppDto, CancellationToken cancellationToken)
    {
        if (chosenAppDto.AppId.HasValue)
        {
            var isSuccess = await AddLink(callbackQuery, chosenAppDto.AppId.Value, cancellationToken);

            if (!isSuccess)
                return;
        }

        await TelegramNotificationService.SendTrackedApps(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            telegramUserId: callbackQuery.From.Id,
            pageInfo: chosenAppDto,
            action: AppAction.Add,
            cancellationToken: cancellationToken);
    }

    private async Task OnChosenRemoveAppCallback(CallbackQuery callbackQuery, ChosenAppCallbackDto chosenAppDto, CancellationToken cancellationToken)
    {
        if (chosenAppDto.AppId.HasValue)
        {
            var isSuccess = await RemoveLink(callbackQuery, chosenAppDto.AppId.Value, cancellationToken);

            if (!isSuccess)
                return;
        }

        await TelegramNotificationService.SendTrackedApps(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            telegramUserId: callbackQuery.From.Id,
            pageInfo: chosenAppDto,
            action: AppAction.Remove,
            cancellationToken: cancellationToken);
    }

    private async Task<bool> AddLink(CallbackQuery callbackQuery, long appId, CancellationToken cancellationToken)
    {
        var (isSuccess, errorMessage) = await UserAppTrackingService
            .LinkUserAndApplication(callbackQuery.From.Id, appId);

        await BotClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: isSuccess ? TelegramBotMessages.ApplicationWasAdded : errorMessage,
            cancellationToken: cancellationToken);

        return isSuccess;
    }

    private async Task<bool> RemoveLink(CallbackQuery callbackQuery, long appId, CancellationToken cancellationToken)
    {
        var (isSuccess, errorMessage) = await UserAppTrackingService
            .RemoveLinkBetweenUserAndApplication(callbackQuery.From.Id, appId);

        await BotClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: isSuccess ? TelegramBotMessages.ApplicationWasDeleted : errorMessage,
            cancellationToken: cancellationToken);

        return isSuccess;
    }
}