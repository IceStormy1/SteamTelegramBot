using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Interfaces;

public interface IUserService
{
    Task AddOrUpdateUser(User telegramUser, long chatId);
}