using SteamTelegramBot.Common.Constants;

namespace SteamTelegramBot.Abstractions.Models.Callbacks;

/// <summary>
/// Dto for adding application callback
/// </summary>
public sealed class AddAppCallbackDto : BaseCallbackDto
{
    /// <inheritdoc cref="BaseCallbackDto.Name"/>
    public override string Name => TelegramCallbacks.AddAppCallback;
}