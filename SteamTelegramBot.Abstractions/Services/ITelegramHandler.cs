using Telegram.Bot.Types;

namespace SteamTelegramBot.Abstractions.Services;

public interface ITelegramHandler
{
    public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}