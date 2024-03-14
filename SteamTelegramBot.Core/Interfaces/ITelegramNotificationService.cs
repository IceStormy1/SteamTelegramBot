using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Interfaces;

public interface ITelegramNotificationService
{
    Task NotifyUsersOfPriceDrop(List<int> applicationIds);
    Task<Message> SendStartInlineKeyBoard(long chatId, CancellationToken cancellationToken, int? messageId = null);
    Task<Message> SendTrackedApps(long chatId, int messageId, long telegramUserId, CancellationToken cancellationToken);
}