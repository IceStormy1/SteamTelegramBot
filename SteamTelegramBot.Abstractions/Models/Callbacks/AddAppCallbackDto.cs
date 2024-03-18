using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class AddAppCallbackDto : BaseCallbackDto
{
    public override string Name => TelegramConstants.AddAppCallback;
}