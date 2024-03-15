using Newtonsoft.Json;
using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

internal class ChosenAppCallback : BaseCallback
{
    public ChosenAppCallback(
        ITelegramBotClient botClient, 
        IUserAppTrackingService userAppTrackingService,
        ITelegramNotificationService telegramNotificationService) : base(botClient, userAppTrackingService, telegramNotificationService)
    {
    }

    public override string Name => TelegramConstants.ChosenAppCallback;

    public override async Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chosenAppDto = JsonConvert.DeserializeObject<ChosenAppCallbackDto>(callbackQuery.Data ?? string.Empty);
        await OnChosenAppCallback(callbackQuery, chosenAppDto, cancellationToken);
    }

    private async Task OnChosenAppCallback(CallbackQuery callbackQuery, ChosenAppCallbackDto chosenAppDto, CancellationToken cancellationToken)
    {
        switch (chosenAppDto.Action)
        {
            case AppAction.Add:
                await OnChosenAddAppCallback(callbackQuery, chosenAppDto.AppId, cancellationToken);
                break;

            case AppAction.Remove:
                await OnChosenRemoveAppCallback(callbackQuery, chosenAppDto.AppId, cancellationToken);
                break;
        }
    }

    private async Task OnChosenAddAppCallback(CallbackQuery callbackQuery, long appId, CancellationToken cancellationToken)
    {
        var (isSuccess, errorMessage) = await UserAppTrackingService.LinkUserAndApplication(callbackQuery.From.Id, appId);

        var messageText = isSuccess ? "Игра добавлена в список" : errorMessage;

        if (isSuccess)
        {
            await TelegramNotificationService.SendTrackedApps(
                chatId: callbackQuery.Message!.Chat.Id,
                messageId: callbackQuery.Message.MessageId,
                telegramUserId: callbackQuery.From.Id,
                cancellationToken: cancellationToken);
        }

        await BotClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: messageText,
            cancellationToken: cancellationToken,
            showAlert: true);
    }

    private async Task OnChosenRemoveAppCallback(CallbackQuery callbackQuery, long appId, CancellationToken cancellationToken)
    {
        await UserAppTrackingService.RemoveLinkBetweenUserAndApplication(callbackQuery.From.Id, appId);

        var trackedApps = await UserAppTrackingService.GetAllUserTrackedApps(callbackQuery.From.Id);

        await BotClient.EditMessageReplyMarkupAsync(
            callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByAppAction(trackedApps, AppAction.Remove),
            cancellationToken: cancellationToken);
        
        await BotClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: "Игра удалена из списка",
            cancellationToken: cancellationToken);
    }
}