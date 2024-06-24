using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Services;

internal sealed class UserService(
    IMapper mapper,
    ILogger<UserService> logger,
    IUserRepository userRepository)
    : BaseService(mapper, logger), IUserService
{
    public async Task AddOrUpdateUser(User telegramUser, long chatId)
    {
        var userEntity = await userRepository.GetUserByTelegramId(telegramUser.Id);

        if (userEntity is null)
        {
            userEntity = Mapper.Map<UserEntity>(telegramUser);
            userEntity.TelegramId = telegramUser.Id;
            userEntity.TelegramChatId = chatId;

            await userRepository.Add(userEntity);
        }
        else
        {
            await userRepository.UpdateUser(telegramUser, userEntity, chatId);
        }
    }
}