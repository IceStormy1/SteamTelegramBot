using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SteamTelegramBot.Abstractions.Exceptions;
using SteamTelegramBot.Abstractions.Models.Callbacks;
using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Core.Callbacks;
using SteamTelegramBot.Core.Commands;
using SteamTelegramBot.Core.Extensions;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramHandleService : BaseService, ITelegramHandleService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserService _userService;
    private readonly ITelegramNotificationService _telegramNotificationService;
    private readonly List<BaseCallback> _telegramCallbacks;
    private readonly List<BaseCommand> _telegramCommands;

    public TelegramHandleService(
        IMapper mapper,
        ILogger<TelegramHandleService> logger,
        ITelegramBotClient botClient,
        IUserService userService,
        ITelegramNotificationService telegramNotificationService,
        IEnumerable<BaseCallback> telegramCallbacks,
        IEnumerable<BaseCommand> telegramCommands) : base(mapper, logger)
    {
        _botClient = botClient;
        _userService = userService;
        _telegramNotificationService = telegramNotificationService;
        _telegramCommands = telegramCommands.ToList();
        _telegramCallbacks = telegramCallbacks.ToList();
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
                await ExecuteCommand(message, cancellationToken);
                break;

            case { EditedMessage: { } message }:
                await ExecuteCommand(message, cancellationToken);
                break;

            case { CallbackQuery: { } callbackQuery }:
                await ExecuteCallback(callbackQuery, cancellationToken);
                break;

            default:
                UnknownUpdateHandlerAsync();
                break;
        }
    }

    private async Task ExecuteCommand(Message message, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Receive message type: {MessageType}", message.Type);

        await _userService.AddOrUpdateUser(message.From, message.Chat.Id);

        if (message.Text is not { } messageText)
            return;

        var commandName = messageText.Split(' ')[0];

        var command = _telegramCommands.FirstOrDefault(x => x.Name == commandName);
        if (command is null)
        {
            await UnknownCommand(message, cancellationToken);
            return;
        }

        await command.Execute(message, cancellationToken);
    }

    private async Task ExecuteCallback(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var callbackDto = JsonConvert.DeserializeObject<BaseCallbackDto>(callbackQuery.Data ?? string.Empty);

        var callback = _telegramCallbacks.FirstOrDefault(x => string.Equals(x.Name, callbackDto?.Name));

        if (callback is null)
        {
            await UnknownCommand(callbackQuery.Message, cancellationToken);
            return;
        }

        await callback.Execute(callbackQuery, cancellationToken);
    }

    private async Task UnknownCommand(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Неизвестная команда. Попробуйте ещё раз",
            cancellationToken: cancellationToken);

        await _telegramNotificationService.SendStartInlineKeyBoard(message.Chat.Id, cancellationToken);
    }

    private void UnknownUpdateHandlerAsync()
        => Logger.LogWarning("Unknown type");
}