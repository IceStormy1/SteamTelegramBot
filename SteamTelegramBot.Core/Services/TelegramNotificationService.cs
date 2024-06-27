using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Abstractions.Models.Applications;
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
using Telegram.Bot.Types.ReplyMarkups;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramNotificationService(
    IMapper mapper,
    ILogger<TelegramNotificationService> logger,
    ITelegramBotClient botClient,
    ITelegramNotificationRepository telegramNotificationRepository,
    IUserAppTrackingService userAppTrackingService)
    : BaseService(mapper, logger), ITelegramNotificationService
{
    private const byte LimitOfSentMessages = 30;
    private const byte TelegramMessageDelayInSeconds = 1;

    public async Task NotifyUsersOfPriceDrop(List<int> applicationIds)
    {
        var unNotifiedUsers = await telegramNotificationRepository.GetUnNotifiedUsers(applicationIds);

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

        logger.LogInformation("Sent {TotalSentMessages} messages to telegram users", totalSentMessages);
    }

    public async Task<Message> SendStartInlineKeyBoard(long chatId, CancellationToken cancellationToken, int? messageId = null)
    {
        const string text = "Для взаимодействия с ботом нажмите на соответствующую кнопку:";

        if (messageId.HasValue)
        {
            return await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: messageId.Value,
                text: text,
                replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.Start),
                cancellationToken: cancellationToken);
        }

        return await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            replyMarkup: InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.Start),
            cancellationToken: cancellationToken,
            disableNotification: true);
    }

    public async Task SendTrackedApps(
        long chatId, 
        int messageId,
        long telegramUserId,
        IPaged pageInfo,
        AppAction action, 
        CancellationToken cancellationToken)
    {
        if (pageInfo.Ignore.HasValue && pageInfo.Ignore.Value)
            return;

        var trackedApps = await GetUserTrackedAppsAsync(telegramUserId, pageInfo);

        await UpdatePageInfoIfNeeded(telegramUserId, pageInfo, trackedApps);

        var text = GenerateMessageText(action, trackedApps, pageInfo);

        var replyMarkup = GenerateReplyMarkup(trackedApps, pageInfo, action);

        await SendMessageAsync(chatId, messageId, text, pageInfo.Current, replyMarkup, cancellationToken);
    }

    private Task<ListResponseDto<TrackedAppItemDto>> GetUserTrackedAppsAsync(long telegramUserId, IPaged pageInfo)
        => userAppTrackingService.GetUserTrackedApps(
            telegramUserId: telegramUserId,
            limit: pageInfo.Size,
            offset: (pageInfo.Current - 1) * pageInfo.Size);

    private async Task UpdatePageInfoIfNeeded(long telegramUserId, IPaged pageInfo, ListResponseDto<TrackedAppItemDto> trackedApps)
    {
        pageInfo.Total = (trackedApps.Total + pageInfo.Size - 1) / pageInfo.Size;
        if (pageInfo.Current > pageInfo.Total)
        {
            pageInfo.Current = pageInfo.Total;
            trackedApps.Items = (await GetUserTrackedAppsAsync(telegramUserId, pageInfo)).Items;
        }
    }

    private static string GenerateMessageText(AppAction action, ListResponseDto<TrackedAppItemDto> trackedApps, IPaged pageInfo)
    {
        if (pageInfo.Total == default)
            return "Пусто";

        return action == AppAction.Remove
            ? TelegramBotMessages.RemoveGameCallbackMessage
            : "Список отслеживаемых игр:";
    }

    private static InlineKeyboardMarkup GenerateReplyMarkup(ListResponseDto<TrackedAppItemDto> trackedApps, IPaged pageInfo, AppAction action)
        => pageInfo.Total == default
            ? InlineKeyBoardHelper.GetInlineKeyboardByType(InlineKeyBoardType.BackToMainMenu)
            : InlineKeyBoardHelper.GetPagedInlineKeyboardByAppAction(trackedApps.Items, pageInfo, action);

    private async Task SendMessageAsync(long chatId, int messageId, string text, int currentPage, InlineKeyboardMarkup replyMarkup, CancellationToken cancellationToken)
    {
        if (currentPage <= IPaged.DefaultPage)
        {
            await botClient.EditMessageTextAsync(
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
            await botClient.EditMessageReplyMarkupAsync(
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
        await botClient.SendTextMessageAsync(chatId: chatId, text: text, parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
        logger.LogInformation("The notification was sent to a user with an ID {ChatId}", chatId);
    }

    private async Task MarkMessageAsSent(List<TelegramNotificationEntity> telegramNotifications)
    {
        telegramNotifications.ForEach(notification=>notification.WasSent = true);
        await telegramNotificationRepository.UpdateRange(telegramNotifications);
    }
}