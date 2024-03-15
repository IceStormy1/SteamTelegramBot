using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class RemoveAppCallbackDto : BaseCallbackDto
{
    public override string CallbackName => TelegramConstants.RemoveAppCallback;
}