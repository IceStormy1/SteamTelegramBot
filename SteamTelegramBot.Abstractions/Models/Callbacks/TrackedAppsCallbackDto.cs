using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class TrackedAppsCallbackDto : PagedCallbackDto
{
    public override string Name => TelegramConstants.TrackedAppsCallback;
}