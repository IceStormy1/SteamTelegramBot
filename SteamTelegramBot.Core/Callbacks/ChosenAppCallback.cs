using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

internal class ChosenAppCallback : BaseCallback
{
    public override string Name => TelegramCallbacks.ChosenAppCallback;

    public ChosenAppCallback(
        ITelegramBotClient botClient, 
        IUserAppTrackingService userAppTrackingService,
        ITelegramNotificationService telegramNotificationService) : base(botClient, userAppTrackingService, telegramNotificationService)
    {
    }

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
            text: isSuccess ? "Игра добавлена в список" : errorMessage,
            cancellationToken: cancellationToken);

        return isSuccess;
    }

    private async Task<bool> RemoveLink(CallbackQuery callbackQuery, long appId, CancellationToken cancellationToken)
    {
        var (isSuccess, errorMessage) = await UserAppTrackingService
            .RemoveLinkBetweenUserAndApplication(callbackQuery.From.Id, appId);

        await BotClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: isSuccess ? "Игра удалена из списка" : errorMessage,
            cancellationToken: cancellationToken);

        return isSuccess;
    }
}