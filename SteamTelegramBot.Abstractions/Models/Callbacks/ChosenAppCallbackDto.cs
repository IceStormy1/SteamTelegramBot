using SteamTelegramBot.Common.Constants;
using SteamTelegramBot.Common.Enums;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

public sealed class ChosenAppCallbackDto : BaseCallbackDto
{
    public override string CallbackName => TelegramConstants.ChosenAppCallback;

    public long AppId { get; set; }
    public AppAction Action { get; init; }
}