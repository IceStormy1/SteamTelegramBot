using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Interfaces;

/// <summary>
/// Defines the interface for handling Telegram updates.
/// </summary>
public interface ITelegramHandleService
{
    /// <summary>
    /// Handles the incoming Telegram update.
    /// </summary>
    /// <param name="update">The Telegram update to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}