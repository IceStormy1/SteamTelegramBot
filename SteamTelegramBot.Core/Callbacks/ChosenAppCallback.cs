using SteamTelegramBot.Abstractions.Exceptions;
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
        var actionTypeString = callbackQuery.Data?.Split(' ')
            .FirstOrDefault(x => x == AppAction.Get.ToString() || x == AppAction.Remove.ToString());

        if (!string.IsNullOrWhiteSpace(actionTypeString)
            && Enum.TryParse<AppAction>(actionTypeString, out var actionType))
        {
            await OnChosenAppCallback(callbackQuery, actionType, cancellationToken); 
        }
    }

    private async Task OnChosenAppCallback(CallbackQuery callbackQuery, AppAction appAction, CancellationToken cancellationToken)
    {
        var appId = callbackQuery.Data?
            .Split(new[] { AppAction.Get.ToString(), AppAction.Remove.ToString() }, StringSplitOptions.RemoveEmptyEntries)
            .ElementAtOrDefault(1);

        if (!int.TryParse(appId, out var parsedAppId))
            throw new TelegramException(chatId: callbackQuery.Message!.Chat.Id, "Не удалось совершить операцию");

        switch (appAction)
        {
            case AppAction.Get:
                await OnChosenAddAppCallback(callbackQuery, parsedAppId, cancellationToken);
                break;

            case AppAction.Remove:
                await OnChosenRemoveAppCallback(callbackQuery, parsedAppId, cancellationToken);
                break;
        }
    }

    private async Task OnChosenAddAppCallback(CallbackQuery callbackQuery, int appId, CancellationToken cancellationToken)
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

    private async Task OnChosenRemoveAppCallback(CallbackQuery callbackQuery, int appId, CancellationToken cancellationToken)
    {
        await UserAppTrackingService.RemoveLinkBetweenUserAndApplication(callbackQuery.From.Id, appId);

        var trackedApps = await UserAppTrackingService.GetUserTrackedApps(callbackQuery.From.Id);

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