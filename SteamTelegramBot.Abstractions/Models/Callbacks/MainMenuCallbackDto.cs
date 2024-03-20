using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class MainMenuCallbackDto : BaseCallbackDto
{
    public override string Name => TelegramCallbacks.MainMenuCallback;
}