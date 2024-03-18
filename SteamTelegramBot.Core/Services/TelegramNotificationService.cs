using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Common.Constants;
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
    private const byte LimitOfSentMessages = 30;
    private const byte TelegramMessageDelayInSeconds = 1;

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

        var totalSentMessages = 0;

        while (true)
        {
            var batchUnNotifiedUsers = unNotifiedUsers.Skip(totalSentMessages)
                .Take(LimitOfSentMessages)
                .ToList();

            if(batchUnNotifiedUsers.Count == default)
                break;

            var tasks = batchUnNotifiedUsers.Select(pair => NotifyUser(telegramChatId: pair.Key, discountedApps: pair.Value));
            await Task.WhenAll(tasks);

            // Telegram doc: https://core.telegram.org/bots/faq#my-bot-is-hitting-limits-how-do-i-avoid-this
            await Task.Delay(TimeSpan.FromSeconds(TelegramMessageDelayInSeconds));

            totalSentMessages += LimitOfSentMessages;
        }

        var notifications = unNotifiedUsers.SelectMany(x => x.Value).ToList();
        await MarkMessageAsSent(notifications);

        _logger.LogInformation("Sent {TotalSentMessages} messages to telegram users", totalSentMessages);
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

    public async Task SendTrackedApps(long chatId, int messageId, long telegramUserId, IPaged pageInfo, AppAction action, CancellationToken cancellationToken)
    {
        if (pageInfo.Ignore.HasValue && pageInfo.Ignore.Value)
            return;

        var text = action == AppAction.Remove
            ? TelegramConstants.RemoveGameCallbackMessage
            : "Список отслеживаемых игр:";

        var trackedApps = await _userAppTrackingService.GetUserTrackedApps(
            telegramUserId: telegramUserId, 
            limit: pageInfo.Size,
            offset: (pageInfo.Current - 1) * pageInfo.Size);
        
        // TODO: Баг при удалении последнего приложения со страницы
        // TODO: Отправлять "пусто", если приложений нет и это первая страница

        pageInfo.Total = (int)Math.Ceiling((double)trackedApps.Total / (double)pageInfo.Size);
        var replyMarkup = InlineKeyBoardHelper.GetPagedInlineKeyboardByAppAction(trackedApps.Items, pageInfo, action);

        if (pageInfo.Current == IPaged.DefaultPage)
        {
            await _botClient.EditMessageTextAsync(
                chatId,
                messageId,
                text: text,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken,
                disableWebPagePreview: true,
                parseMode: ParseMode.MarkdownV2);
        }
        else
        {
            await _botClient.EditMessageReplyMarkupAsync(
                chatId,
                messageId,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }
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