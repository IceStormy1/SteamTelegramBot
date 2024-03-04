using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Extensions;
using SteamTelegramBot.Data.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramNotificationService : BaseService, ITelegramNotificationService
{
    private const byte LimitOfSentMessages = 30;
    private const byte TelegramMessageDelayInSeconds = 1;

    private readonly ITelegramBotClient _botClient;
    private readonly ITelegramNotificationRepository _telegramNotificationRepository;
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(
        IMapper mapper, 
        ILogger<TelegramNotificationService> logger, 
        ITelegramBotClient botClient,
        ITelegramNotificationRepository telegramNotificationRepository, 
        ILogger<TelegramNotificationService> logger1) : base(mapper, logger)
    {
        _botClient = botClient;
        _telegramNotificationRepository = telegramNotificationRepository;
        _logger = logger1;
    }

    public async Task NotifyUsersOfPriceDrop()
    {
        var totalSentMessages = 0;

        while (true)
        {
            var unNotifiedUsers = await _telegramNotificationRepository.GetUnNotifiedUsers(limit: LimitOfSentMessages, totalSentMessages);

            if(unNotifiedUsers.Count == default)
                break;

            var tasks = unNotifiedUsers.Select(pair => NotifyUser(pair.Key, pair.Value));
            await Task.WhenAll(tasks);

            var notifications = unNotifiedUsers.SelectMany(x => x.Value).ToList();
            await MarkMessageAsSent(notifications);

            // Telegram doc: https://core.telegram.org/bots/faq#my-bot-is-hitting-limits-how-do-i-avoid-this
            await Task.Delay(TimeSpan.FromSeconds(TelegramMessageDelayInSeconds));

            totalSentMessages += unNotifiedUsers.Count;
        }

        _logger.LogInformation("Sent {TotalSentMessages} messages to telegram users", totalSentMessages);
    }

    private async Task NotifyUser(long telegramChatId, List<TelegramNotificationEntity> discountedApps)
    {
        var formattedListOfDiscountedApps = discountedApps.Select((item, index) => item.ToTelegramHyperlink(index));

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