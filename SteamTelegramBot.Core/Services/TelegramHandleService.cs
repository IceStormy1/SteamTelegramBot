using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Core.Callbacks;
using SteamTelegramBot.Core.Extensions;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramHandleService : BaseService, ITelegramHandleService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserService _userService;
    private readonly ISteamService _steamService;
    private readonly ITelegramNotificationService _telegramNotificationService;
    private readonly List<BaseCallback> _callbacks;

    public TelegramHandleService(
        IMapper mapper,
        ILogger<TelegramHandleService> logger,
        ITelegramBotClient botClient,
        IUserService userService,
        ISteamService steamService,
        ITelegramNotificationService telegramNotificationService,
        IEnumerable<BaseCallback> callbacks) : base(mapper, logger)
    {
        _botClient = botClient;
        _userService = userService;
        _steamService = steamService;
        _telegramNotificationService = telegramNotificationService;
        _callbacks = callbacks.ToList();
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
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
                await ExecuteCallback(callbackQuery, cancellationToken);
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
            TelegramCommands.AddGameCommand => await ProcessAddGameCommand(string.Join(" ", splitMessage, 1, splitMessage.Length - 1), message.Chat.Id, cancellationToken),
            _ => await UnknownCommand(message, cancellationToken),
        };

        Logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task ExecuteCallback(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var callbackDto = JsonConvert.DeserializeObject<BaseCallbackDto>(callbackQuery.Data ?? string.Empty);

        var callback = _callbacks.FirstOrDefault(x => x.Name == callbackDto?.Name);

        if (callback is null)
        {
            await UnknownCommand(callbackQuery.Message, cancellationToken);
            return;
        }

        await callback.Execute(callbackQuery, cancellationToken);
    }

    private async Task<Message> SendStartMessage(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: TelegramConstants.StartMessage,
            cancellationToken: cancellationToken);

        return await _telegramNotificationService.SendStartInlineKeyBoard(chatId: message.Chat.Id, cancellationToken);
    }

    private async Task<Message> UnknownCommand(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Неизвестная команда. Попробуйте ещё раз",
            cancellationToken: cancellationToken);

        return await _telegramNotificationService.SendStartInlineKeyBoard(message.Chat.Id, cancellationToken);
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
}