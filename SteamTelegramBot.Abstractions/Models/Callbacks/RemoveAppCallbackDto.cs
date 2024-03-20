using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

/// <summary>
/// Dto for removing application from tracked list
/// </summary>
public sealed class RemoveAppCallbackDto : PagedCallbackDto
{
    /// <inheritdoc cref="BaseCallbackDto.Name"/>
    public override string Name => TelegramCallbacks.RemoveAppCallback;
}