using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Data.Entities;
using SteamTelegramBot.Data.Interfaces;

namespace SteamTelegramBot.Data.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly SteamTelegramBotDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly IMapper Mapper;

    protected BaseRepository(SteamTelegramBotDbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task Add<TSource>(TSource incoming) where TSource : class
    {
        var entity = Mapper.Map<TEntity>(incoming);
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task Add(TEntity entity)
    {
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task AddRange<TSource>(IReadOnlyCollection<TSource> incoming) where TSource : class
    {
        // TODO: Z.EntityFramework.Extensions.EFCore

        var entities = Mapper.Map<TEntity>(incoming);
        DbContext.AddRange(entities);
        await DbContext.SaveChangesAsync();
    }

    public async Task Update<TSource>(TSource incoming, TEntity existingEntity) where TSource : class
    {
        Mapper.Map(incoming, existingEntity);
        await DbContext.SaveChangesAsync();
    }

    public async Task UpdateRange<TSource>(
        List<TSource> source,
        List<TEntity> entities,
        Func<TSource, IEnumerable<TEntity>, TEntity> find)
        where TSource : class
    {
        Mapper.MapCollection(source: source, target: entities, find: find);

        await DbContext.SaveChangesAsync();
    }
}