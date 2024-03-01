using SteamTelegramBot.Data.Entities;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Data.Interfaces;

public interface IUserRepository : IBaseRepository<UserEntity>
{
    Task<UserEntity> GetUserByTelegramId(long telegramId);
    Task UpdateUser(User telegramUser, UserEntity userEntity, long chatId);
}