using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

/// <summary>
/// Dto for callback when user chosen application (remove or add)
/// </summary>
public sealed class ChosenAppCallbackDto : PagedCallbackDto
{
    /// <inheritdoc cref="BaseCallbackDto.Name"/>
    public override string Name => TelegramCallbacks.ChosenAppCallback;

    /// <summary>
    /// Application id
    /// </summary>
    public long? AppId { get; set; }

    /// <summary>
    /// Action on the selected application 
    /// </summary>
    public AppAction Action { get; init; }
}