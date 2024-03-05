using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Common;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Core.Extensions;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Extensions;
using SteamTelegramBot.Data.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramHandleService : BaseService, ITelegramHandleService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserService _userService;
    private readonly ISteamService _steamService;
    private readonly IUserAppTrackingService _userAppTrackingService;
    private readonly IUserAppTrackingRepository _userAppTrackingRepository;

    public TelegramHandleService(
        IMapper mapper,
        ILogger<TelegramHandleService> logger,
        ITelegramBotClient botClient,
        IUserService userService,
        IUserAppTrackingRepository userAppTrackingRepository, 
        ISteamService steamService,
        IUserAppTrackingService userAppTrackingService) : base(mapper, logger)
    {
        _botClient = botClient;
        _userService = userService;
        _userAppTrackingRepository = userAppTrackingRepository;
        _steamService = steamService;
        _userAppTrackingService = userAppTrackingService;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        // TODO: Добавить в сообщение /start надпись о том, что бот предназначен только для РФ
        try
        {
            await HandleUpdate(update, cancellationToken);
        }
        catch (Exception e)
        {
            var chatId = update.GetChatIdFromRequest();

            if (chatId.HasValue)
                throw new TelegramException(e, chatId.Value, AbstractConstants.InternalError);
        }
    }

    private async Task HandleUpdate(Update update, CancellationToken cancellationToken)
    {
        switch (update)
        {
            case { Message: { } message }:
                await BotOnMessageReceived(message, cancellationToken);
                break;

            case { EditedMessage: { } message }:
                await BotOnMessageReceived(message, cancellationToken);
                break;

            case { CallbackQuery: { } callbackQuery }:
                await BotOnCallbackQueryReceived(callbackQuery, cancellationToken);
                break;

            default:
                UnknownUpdateHandlerAsync();
                break;
        }
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Receive message type: {MessageType}", message.Type);

        await _userService.AddOrUpdateUser(message.From, message.Chat.Id);

        if (message.Text is not { } messageText)
            return;

        var splitMessage = messageText.Split(' ');
        var command = splitMessage[0];

        var sentMessage = command switch
        {
            TelegramCommands.StartCommand => await SendStartMessage(message, cancellationToken),
            TelegramCommands.AddGameCommand => await ProcessAddGameCommand(string.Join(" ", splitMessage, 1, splitMessage.Length -1), message.Chat.Id, cancellationToken),
            _ => await UnknownCommand(message, cancellationToken),
        };

        Logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        switch (callbackQuery.Data)
        {
            case TelegramConstants.AddAppCallback:
                await OnAddAppCallback(callbackQuery, cancellationToken);
                return;

            case TelegramConstants.RemoveAppCallback:
                await OnRemoveAppCallback(callbackQuery, cancellationToken);
                return;

            case TelegramConstants.MainMenuCallback:
                await SendStartInlineKeyBoard(
                    chatId: callbackQuery.Message!.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    cancellationToken: cancellationToken);
                return;

            case TelegramConstants.FollowedAppsCallback:
                await OnTrackedAppsCallback(callbackQuery,  cancellationToken);
                return;

            default:
                var actionTypeString = callbackQuery.Data?.Split(' ')
                    .FirstOrDefault(x => x == AppAction.Add.ToString() || x == AppAction.Remove.ToString());

                if (!string.IsNullOrWhiteSpace(actionTypeString) 
                    && Enum.TryParse<AppAction>(actionTypeString, out var actionType))
                {
                    await OnChosenAppCallback(callbackQuery, actionType, cancellationToken);
                    return;
                }

                await UnknownCommand(callbackQuery.Message, cancellationToken);
                return;
        }
    }

    private Task OnAddAppCallback(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        => _botClient.EditMessageTextAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            text: TelegramConstants.AddGameCallbackMessage,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.AddGame),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

    private async Task OnRemoveAppCallback(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var trackedApps = await GetUserTrackedApps(callbackQuery.From.Id);

        await _botClient.EditMessageTextAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            text: TelegramConstants.RemoveGameCallbackMessage,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByAppAction(trackedApps, AppAction.Remove),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private async Task<Message> SendStartMessage(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramConstants.StartMessage,
            cancellationToken: cancellationToken);

        return await SendStartInlineKeyBoard(chatId: message.Chat.Id, cancellationToken);
    }

    private async Task OnTrackedAppsCallback(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var trackedApps = await GetUserTrackedApps(callbackQuery.From.Id);

        await _botClient.EditMessageTextAsync(
            callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            text: trackedApps.Count > 0 ? "Список отслеживаемых игр:" : "Пусто",
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByAppAction(trackedApps, AppAction.Add),
            cancellationToken: cancellationToken,
            disableWebPagePreview: true,
            parseMode: ParseMode.MarkdownV2);
    }

    private async Task OnChosenAppCallback(CallbackQuery callbackQuery, AppAction appAction, CancellationToken cancellationToken)
    {
        var appId = callbackQuery.Data?
            .Split(new[] { AppAction.Add.ToString(), AppAction.Remove.ToString() }, StringSplitOptions.RemoveEmptyEntries)
            .ElementAtOrDefault(0);

        if (!int.TryParse(appId, out var parsedAppId))
            throw new TelegramException(chatId: callbackQuery.Message!.Chat.Id, "Не удалось совершить операцию");

        switch (appAction)
        {
            case AppAction.Add:
                await OnChosenAddAppCallback(callbackQuery, parsedAppId, cancellationToken);
                break;

            case AppAction.Remove:
                await OnChosenRemoveAppCallback(callbackQuery, parsedAppId, cancellationToken);
                break;
        }
    }

    private async Task OnChosenAddAppCallback(CallbackQuery callbackQuery, int appId, CancellationToken cancellationToken)
    {
        var (isSuccess, errorMessage) = await _userAppTrackingService.LinkUserAndApplication(callbackQuery.From.Id, appId);

        var messageText = isSuccess ? "Игра добавлена в список" : errorMessage;

        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: messageText,
            cancellationToken: cancellationToken);

        if (isSuccess)
            await OnTrackedAppsCallback(callbackQuery, cancellationToken);
    }

    private async Task OnChosenRemoveAppCallback(CallbackQuery callbackQuery, int appId, CancellationToken cancellationToken)
    {
        await _userAppTrackingService.RemoveLinkBetweenUserAndApplication(callbackQuery.From.Id, appId);

        var trackedApps = await GetUserTrackedApps(callbackQuery.From.Id);

        await _botClient.EditMessageReplyMarkupAsync(
            callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByAppAction(trackedApps, AppAction.Remove),
            cancellationToken: cancellationToken);
    }

    private async Task<List<TrackedAppItemDto>> GetUserTrackedApps(long telegramUserId)
    {
        var trackedApps = await _userAppTrackingRepository.GetTrackedApplicationsByTelegramId(telegramUserId);
        return trackedApps.Select((app, index) => app.ToTrackedAppItem(index)).ToList();
    }

    private async Task<Message> UnknownCommand(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Неизвестная команда. Попробуйте ещё раз",
            cancellationToken: cancellationToken);

        return await SendStartInlineKeyBoard(message.Chat.Id, cancellationToken);
    }

    private async Task<Message> ProcessAddGameCommand(string appName, long chatId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(appName))
        {
            return await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: TelegramConstants.NeedToApplicationName,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }

        var steamApps = await _steamService.GetSteamSuggests(appName, filterByExistingApps: true);

        if (steamApps.Count == default)
        {
            return await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: TelegramConstants.GameNotFound,
                cancellationToken: cancellationToken);
        }

        return await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Выберите один из вариантов, нажав на соответствующую кнопку",
            replyMarkup: InlineKeyBoardHelper.GetAddGameInlineKeyboard(steamApps),
            cancellationToken: cancellationToken
        );
    }

    private void UnknownUpdateHandlerAsync()
        => Logger.LogWarning("Unknown type");

    private async Task<Message> SendStartInlineKeyBoard(long chatId, CancellationToken cancellationToken,
        int? messageId = null)
    {
        const string text = "Для взаимодействия с ботом нажмите на соответствующую кнопку:";

        if (messageId.HasValue)
        {
            return await _botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId.Value,
                text: text,
                replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.Start),
                cancellationToken: cancellationToken);
        }

        return await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.Start),
            cancellationToken: cancellationToken,
            disableNotification: true);
    }
}