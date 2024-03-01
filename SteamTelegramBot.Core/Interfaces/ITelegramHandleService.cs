using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Interfaces;

public interface ITelegramHandleService
{
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}