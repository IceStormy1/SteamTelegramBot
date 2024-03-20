using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Data.Interfaces;

/// <summary>
/// Represents a generic repository interface for entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Adds the specified entity to the database.
    /// </summary>
    /// <param name="incoming">The incoming entity to add.</param>
    /// <typeparam name="TSource">The type of the incoming entity.</typeparam>
    Task Add<TSource>(TSource incoming) where TSource : class;

    /// <summary>
    /// Adds the specified entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    Task Add(TEntity entity);

    /// <summary>
    /// Adds a collection of entities to the database.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    Task AddRange(IReadOnlyCollection<TEntity> entities);

    /// <summary>
    /// Updates an existing entity in the database based on the incoming data.
    /// </summary>
    /// <param name="incoming">The incoming data used for updating.</param>
    /// <param name="existingEntity">The existing entity to update.</param>
    /// <typeparam name="TSource">The type of the incoming data.</typeparam>
    Task Update<TSource>(TSource incoming, TEntity existingEntity) where TSource : class;

    /// <summary>
    /// Updates a range of entities in the database based on the incoming data.
    /// </summary>
    /// <param name="source">The list of incoming data.</param>
    /// <param name="entities">The list of entities to update.</param>
    /// <param name="find">A function to find existing entities.</param>
    /// <typeparam name="TSource">The type of the incoming data.</typeparam>
    Task UpdateRange<TSource>(
        List<TSource> source,
        List<TEntity> entities,
        Func<TSource, IEnumerable<TEntity>, TEntity> find)
        where TSource : class;

    /// <summary>
    /// Updates a range of entities in the database.
    /// </summary>
    /// <param name="entities">The list of entities to update.</param>
    Task UpdateRange(List<TEntity> entities);

    /// <summary>
    /// Removes the specified entity from the database.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    Task Remove(TEntity entity);
}