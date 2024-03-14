using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Core.Helpers;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Extensions;
using SteamTelegramBot.Data.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramNotificationService : BaseService, ITelegramNotificationService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITelegramNotificationRepository _telegramNotificationRepository;
    private readonly IUserAppTrackingService _userAppTrackingService;
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(
        IMapper mapper, 
        ILogger<TelegramNotificationService> logger, 
        ITelegramBotClient botClient,
        ITelegramNotificationRepository telegramNotificationRepository,
        IUserAppTrackingService userAppTrackingService) : base(mapper, logger)
    {
        _botClient = botClient;
        _telegramNotificationRepository = telegramNotificationRepository;
        _logger = logger;
        _userAppTrackingService = userAppTrackingService;
    }

    public async Task NotifyUsersOfPriceDrop(List<int> applicationIds)
    {
        var unNotifiedUsers = await _telegramNotificationRepository.GetUnNotifiedUsers(applicationIds);

        if (unNotifiedUsers.Count == default)
            return;

        var tasks = unNotifiedUsers.Select(pair => NotifyUser(pair.Key, pair.Value));
        await Task.WhenAll(tasks);

        var notifications = unNotifiedUsers.SelectMany(x => x.Value).ToList();
        await MarkMessageAsSent(notifications);

        _logger.LogInformation("Sent {TotalSentMessages} messages to telegram users", unNotifiedUsers.Count);
    }

    public async Task<Message> SendStartInlineKeyBoard(long chatId, CancellationToken cancellationToken, int? messageId = null)
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

    public async Task<Message> SendTrackedApps(long chatId, int messageId, long telegramUserId, CancellationToken cancellationToken)
    {
        var trackedApps = await _userAppTrackingService.GetAllUserTrackedApps(telegramUserId);

        return await _botClient.EditMessageTextAsync(
            chatId,
            messageId,
            text: trackedApps.Count > 0 ? "Список отслеживаемых игр:" : "Пусто",
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByAppAction(trackedApps, AppAction.Get),
            cancellationToken: cancellationToken,
            disableWebPagePreview: true,
            parseMode: ParseMode.MarkdownV2);
    }

    private async Task NotifyUser(long telegramChatId, List<TelegramNotificationEntity> discountedApps)
    {
        var formattedListOfDiscountedApps = discountedApps
            .Select((item, index) => item.SteamAppPrice?.SteamApp.ToTelegramHyperlink(index));

        var messageText = FormatDiscountedAppsMessage(formattedListOfDiscountedApps);

        await SendTelegramMessage(telegramChatId, messageText);
    }

    private static string FormatDiscountedAppsMessage(IEnumerable<string> formattedApps)
    {
        var appsList = string.Join("\n", formattedApps);
        return $"Из вашего списка снизилась цена на следующие товары:\n{appsList}";
    }

    private async Task SendTelegramMessage(long chatId, string text)
    {
        text = text.ToTelegramMarkdownMessageText();
        await _botClient.SendTextMessageAsync(chatId: chatId, text: text, parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }

    private async Task MarkMessageAsSent(List<TelegramNotificationEntity> telegramNotifications)
    {
        telegramNotifications.ForEach(notification=>notification.WasSent = true);
        await _telegramNotificationRepository.UpdateRange(telegramNotifications);
    }
}