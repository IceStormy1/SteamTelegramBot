using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Services;

internal sealed class UserService : BaseService, IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(
        IMapper mapper, 
        ILogger<BaseService> logger,
        IUserRepository userRepository) : base(mapper, logger)
    {
        _userRepository = userRepository;
    }

    public async Task AddOrUpdateUser(User telegramUser, long chatId)
    {
        var userEntity = await _userRepository.GetUserByTelegramId(telegramUser.Id);

        if (userEntity is null)
        {
            userEntity = Mapper.Map<UserEntity>(telegramUser);
            userEntity.TelegramId = telegramUser.Id;
            userEntity.TelegramChatId = chatId;

            await _userRepository.Add(userEntity);
        }
        else
        {
            userEntity.TelegramId = userEntity.Id;
            userEntity.TelegramChatId = chatId;
            await _userRepository.UpdateUser(telegramUser, userEntity, chatId);
        }
    }
}