using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class UserRepository(SteamTelegramBotDbContext dbContext, IMapper mapper)
    : BaseRepository<UserEntity>(dbContext, mapper), IUserRepository
{
    public Task<UserEntity> GetUserByTelegramId(long telegramId)
        => DbSet.FirstOrDefaultAsync(x => x.TelegramId == telegramId);

    public async Task UpdateUser(User telegramUser, UserEntity userEntity, long chatId)
    {
        Mapper.Map(telegramUser, userEntity);
        userEntity.TelegramId = telegramUser.Id;
        userEntity.TelegramChatId = chatId;

        await DbContext.SaveChangesAsync();
    }
}