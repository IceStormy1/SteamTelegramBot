using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class RemoveAppCallbackDto : PagedCallbackDto
{
    public override string Name => TelegramConstants.RemoveAppCallback;
}