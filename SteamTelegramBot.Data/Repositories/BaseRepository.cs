namespace SteamTelegramBot.Data.Repositories;

public abstract class BaseRepository
{
    protected readonly SteamTelegramBotDbContext DbContext;

    protected BaseRepository(SteamTelegramBotDbContext dbContext)
    {
        DbContext = dbContext;
    }
}