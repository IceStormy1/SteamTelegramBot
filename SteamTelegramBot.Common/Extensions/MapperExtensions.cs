using AutoMapper;

namespace SteamTelegramBot.Common.Extensions;

public static class MapperExtensions
{
    /// <summary>
    /// Provides extension methods for mapping collections using AutoMapper.
    /// </summary>
    /// <typeparam name="TSource">The type of the source objects.</typeparam>
    /// <typeparam name="TTarget">The type of the target objects.</typeparam>
    /// <param name="mapper">The IMapper instance used for mapping.</param>
    /// <param name="source">The collection of source objects to map.</param>
    /// <param name="target">The list of target objects to populate.</param>
    /// <param name="find">A function to find the target object corresponding to a source object.</param>
    /// <param name="map">An optional mapping function for additional customization.</param>
    public static void MapCollection<TSource, TTarget>(
        this IMapper mapper,
        IReadOnlyCollection<TSource> source,
        List<TTarget> target,
        Func<TSource, IEnumerable<TTarget>, TTarget> find, 
        Func<TSource, TTarget, TTarget> map = null)
    {
        foreach (var item in source)
        {
            var targetItem = find(item, target);
            if (targetItem != null)
            {
                if (map == null)
                    mapper.Map(item, targetItem);
                else
                {
                    _ = map(item, targetItem);
                }
            }
            else
            {
                targetItem = map == null
                    ? mapper.Map<TTarget>(item)
                    : map(item, default);

                target.Add(targetItem);
            }
        }
    }
}