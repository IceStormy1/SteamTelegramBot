using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

/// <summary>
/// Dto for sent main menu
/// </summary>
public sealed class MainMenuCallbackDto : BaseCallbackDto
{
    /// <inheritdoc cref="BaseCallbackDto.Name"/>
    public override string Name => TelegramCallbacks.MainMenuCallback;
}