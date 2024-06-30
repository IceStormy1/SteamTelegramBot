using Microsoft.AspNetCore.Mvc;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Filters;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Controllers;

/// <summary>
/// Telegram bot controller.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class BotController : ControllerBase
{
    private readonly ITelegramHandleService _telegramService;

    /// <inheritdoc cref="BotController"/>
    public BotController(ITelegramHandleService telegramService) => _telegramService = telegramService;

    /// <summary>
    /// Telegram request
    /// </summary>
    /// <param name="update"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Update(
        [FromBody] Update update,
        CancellationToken cancellationToken
    )
    {
        await _telegramService.HandleUpdateAsync(update, cancellationToken);

        return Ok();
    }
}