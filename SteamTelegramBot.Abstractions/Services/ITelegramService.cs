using Telegram.Bot.Types;

namespace SteamTelegramBot.Abstractions.Services;

public interface ITelegramService
{
    public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}