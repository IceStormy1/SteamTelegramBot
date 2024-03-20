using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

/// <summary>
/// Dto for getting tracked applications
/// </summary>
public sealed class TrackedAppsCallbackDto : PagedCallbackDto
{
    /// <inheritdoc cref="BaseCallbackDto.Name"/>
    public override string Name => TelegramCallbacks.TrackedAppsCallback;
}