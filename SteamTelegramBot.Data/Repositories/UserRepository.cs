using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Data.Repositories;

internal sealed class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(SteamTelegramBotDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

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