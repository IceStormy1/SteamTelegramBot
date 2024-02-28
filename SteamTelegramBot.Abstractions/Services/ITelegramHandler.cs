using Telegram.Bot.Types;

namespace SteamTelegramBot.Abstractions.Services;

public interface ITelegramHandler
{
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}