using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class AddAppCallbackDto : BaseCallbackDto
{
    public override string CallbackName => TelegramConstants.AddAppCallback;
}