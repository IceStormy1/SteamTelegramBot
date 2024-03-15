using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class TrackedAppsCallbackDto : BaseCallbackDto
{
    public override string CallbackName => TelegramConstants.TrackedAppsCallback;
}