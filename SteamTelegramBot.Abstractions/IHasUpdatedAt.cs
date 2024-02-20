namespace SteamTelegramBot.Abstractions;

/// <summary>
/// Интерфейс, представляющий сущность, у которой есть дата и время обновления.
/// </summary>
public interface IHasUpdatedAt
{
    /// <summary>
    /// Дата и время изменения сущености
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}