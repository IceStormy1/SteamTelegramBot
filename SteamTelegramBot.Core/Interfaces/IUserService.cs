using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Interfaces;

/// <summary>
/// Defines the interface for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Adds or updates a Telegram user.
    /// </summary>
    /// <param name="telegramUser">The Telegram user to add or update.</param>
    /// <param name="chatId">The chat ID associated with the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddOrUpdateUser(User telegramUser, long chatId);
}