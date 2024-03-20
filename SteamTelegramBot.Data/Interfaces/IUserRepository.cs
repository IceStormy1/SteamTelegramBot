using SteamTelegramBot.Data.Entities;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Data.Interfaces;

/// <summary>
/// Represents a repository interface for users.
/// </summary>
public interface IUserRepository : IBaseRepository<UserEntity>
{
    /// <summary>
    /// Retrieves a user entity by Telegram ID.
    /// </summary>
    /// <param name="telegramId">The Telegram ID of the user.</param>
    /// <returns>The user entity.</returns>
    Task<UserEntity> GetUserByTelegramId(long telegramId);

    /// <summary>
    /// Updates a user entity based on the specified Telegram user and user entity.
    /// </summary>
    /// <param name="telegramUser">The Telegram user.</param>
    /// <param name="userEntity">The user entity to update.</param>
    /// <param name="chatId">The chat ID associated with the user.</param>
    Task UpdateUser(User telegramUser, UserEntity userEntity, long chatId);
}