namespace SteamTelegramBot.Abstractions;

/// <summary>
/// Интерфейс, представляющий сущность, у которой есть дата и время создания.
/// </summary>
public interface IHasCreatedAt
{
    /// <summary>
    /// Дата и время создания сущности
    /// </summary>
    public DateTime CreatedAt { get; set; }
}