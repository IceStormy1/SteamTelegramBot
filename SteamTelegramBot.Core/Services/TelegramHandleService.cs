using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Core.Extensions;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramHandleService : BaseService, ITelegramHandleService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserService _userService;

    public TelegramHandleService(
        IMapper mapper,
        ILogger<BaseService> logger,
        ITelegramBotClient botClient, 
        IUserService userService) : base(mapper, logger)
    {
        _botClient = botClient;
        _userService = userService;
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
                throw new TelegramException(e, chatId.Value, "Произошла непредвиденная ошибка. Попробуйте ещё раз");
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
        
        var sentMessage = messageText.Split(' ')[0] switch
        {
            "/inline_keyboard" => await SendInlineKeyboard(message, cancellationToken),
            "/start" => await Usage(message, cancellationToken),
            _ => await UnknownCommand(message, cancellationToken),
        };
        
        Logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task<Message> SendInlineKeyboard(Message message, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData("1.1", "11"),
                },
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData("2.1", "21"),
                },
                new []
                {
                    InlineKeyboardButton.WithSwitchInlineQuery("test query", "test"),
                },
            });
       
        return await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Чтобы ознакомиться с тарифом, выберите необходимый, нажав на соответствующую кнопку",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        //await _botClient.AnswerCallbackQueryAsync(
        //    callbackQueryId: callbackQuery.Id,
        //    text: $"Received {callbackQuery.Data}",
        //    cancellationToken: cancellationToken);

        InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData("1.1test", "11"),
                },
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData("2.1test", "21"),
                },
                new []
                {
                    InlineKeyboardButton.WithSwitchInlineQuery("test quer1y", "test"),
                },
            });

        //await _botClient.EditMessageTextAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
        //    "edited test", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);

        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);
    }

    private async Task<Message> Usage(Message message, CancellationToken cancellationToken)
    {
        const string usage = "Usage:\n" +
                             "/inline_keyboard - send inline keyboard\n" +
                             "/keyboard    - send custom keyboard\n" +
                             "/remove      - remove custom keyboard\n" +
                             "/request     - request location or contact\n" +
                             "/inline_mode - send keyboard with Inline Query";
      
        return await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: usage,
            cancellationToken: cancellationToken);
    }

    private async Task<Message> UnknownCommand(Message message, CancellationToken cancellationToken)
    {
        return await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Неизвестная команда. Попробуйте ещё раз",
            cancellationToken: cancellationToken);
    }

    private void UnknownUpdateHandlerAsync()
        => Logger.LogWarning("Unknown type");
}