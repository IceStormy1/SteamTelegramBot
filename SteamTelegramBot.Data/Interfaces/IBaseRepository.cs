using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task Add<TSource>(TSource incoming) where TSource : class;
    Task Add(TEntity entity);

    Task AddRange<TSource>(IReadOnlyCollection<TSource> incoming) where TSource : class;

    Task Update<TSource>(TSource incoming, TEntity existingEntity) where TSource : class;

    Task UpdateRange<TSource>(
        List<TSource> source,
        List<TEntity> entities,
        Func<TSource, IEnumerable<TEntity>, TEntity> find)
        where TSource : class;
}