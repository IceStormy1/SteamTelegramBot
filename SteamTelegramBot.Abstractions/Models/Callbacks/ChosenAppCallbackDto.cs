using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class ChosenAppCallbackDto : PagedCallbackDto
{
    public override string Name => TelegramCallbacks.ChosenAppCallback;

    public long? AppId { get; set; }
    public AppAction Action { get; init; }
}