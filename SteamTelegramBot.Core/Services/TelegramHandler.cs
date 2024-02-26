using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamTelegramBot.Core.Services;

public sealed class TelegramHandler : BaseService, ITelegramHandler
{
    private readonly ITelegramBotClient _botClient;

    public TelegramHandler(
        IMapper mapper,
        ILogger<BaseService> logger,
        ITelegramBotClient botClient) : base(mapper, logger)
    {
        _botClient = botClient;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        try
        {
            await HandleUpdate(update, cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "An error occurred while processing the request");
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

            case { InlineQuery: { } inlineQuery }:
                await BotOnInlineQueryReceived(inlineQuery, cancellationToken);
                break;

            case { ChosenInlineResult: { } chosenInlineResult }:
                await BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken);
                break;

            default:
                UnknownUpdateHandlerAsync();
                break;
        }
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { } messageText)
            return;
        
        var sentMessage = messageText.Split(' ')[0] switch
        {
            "/inline_keyboard" => await SendInlineKeyboard(message, cancellationToken),
            _ => await Usage(message, cancellationToken)
        };
        
        Logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
    }

    // Send inline keyboard
    // You can process responses in BotOnCallbackQueryReceived handler
    private async Task<Message> SendInlineKeyboard(Message message, CancellationToken cancellationToken)
    {
        //await botClient.SendChatActionAsync(
        //    chatId: message.Chat.Id,
        //    chatAction: ChatAction.Typing,
        //    cancellationToken: cancellationToken);

        InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                // first row
                new []
                {
                    InlineKeyboardButton.WithCallbackData("1.1", "11"),
                    //InlineKeyboardButton.WithCallbackData("1.2", "12"),
                },
                // second row
                new []
                {
                    InlineKeyboardButton.WithCallbackData("2.1", "21"),
                    
                    //InlineKeyboardButton.WithCallbackData("2.2", "22"),
                },
            });

        return await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Чтобы ознакомиться с тарифом, выберите необходимый, нажав на соответствующую кнопку",
            replyMarkup: inlineKeyboard,
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
            replyMarkup: new ReplyKeyboardRemove(),
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
        
        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);
    }

    #region Inline Mode

    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "1",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent("hello"))
        };

        await _botClient.AnswerInlineQueryAsync(
            inlineQueryId: inlineQuery.Id,
            results: results,
            cacheTime: 0,
            isPersonal: true,
            cancellationToken: cancellationToken);
    }

    private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);

        await _botClient.SendTextMessageAsync(
            chatId: chosenInlineResult.From.Id,
            text: $"You chose result with Id: {chosenInlineResult.ResultId}",
            cancellationToken: cancellationToken);
    }

    #endregion

    private void UnknownUpdateHandlerAsync()
        => Logger.LogWarning("Unknown type");
}