using SteamTelegramBot.Abstractions;
using SteamTelegramBot.Common.Enums;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Interfaces;

/// <summary>
/// Defines the interface for sending notifications and interacting with users in Telegram.
/// </summary>
public interface ITelegramNotificationService
{
    /// <summary>
    /// Notifies users of a price drop for specified applications.
    /// </summary>
    /// <param name="applicationIds">The list of application IDs to notify users about.</param>
    Task NotifyUsersOfPriceDrop(List<int> applicationIds);

    /// <summary>
    /// Sends the start inline keyboard to a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat to send the keyboard to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="messageId">The ID of the message to update, if applicable.</param>
    Task<Message> SendStartInlineKeyBoard(long chatId, CancellationToken cancellationToken, int? messageId = null);

    /// <summary>
    /// Sends tracked applications to a specific chat with paging.
    /// </summary>
    /// <param name="chatId">The ID of the chat to send the tracked applications to.</param>
    /// <param name="messageId">The ID of the message to update.</param>
    /// <param name="telegramUserId">The Telegram user ID.</param>
    /// <param name="pageInfo">The page information.</param>
    /// <param name="action">The action to perform on the applications.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SendTrackedApps(long chatId, int messageId, long telegramUserId, IPaged pageInfo, AppAction action, CancellationToken cancellationToken);
}