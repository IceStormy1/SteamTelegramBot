using AutoMapper;

namespace SteamTelegramBot.Common.Extensions;

public static class MapperExtensions
{
    /// <summary>
    /// Маппит коллекцию на другую поэлементно. Учитываются существующие элементы в <typeparamref name="TTarget"/>
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="source">Источник</param>
    /// <param name="target">Конечный список</param>
    /// <param name="find">Поиск источника в конечном списке</param>
    /// <param name="map">Функция определяемого пользователем маппинга</param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
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