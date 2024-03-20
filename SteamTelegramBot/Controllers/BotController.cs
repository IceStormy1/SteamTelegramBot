using Microsoft.AspNetCore.Mvc;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Clients.Models;
using SteamTelegramBot.Core.Interfaces;
using SteamTelegramBot.Filters;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Controllers;

/// <summary>
/// Telegram bot controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
    private readonly ILogger<BotController> _logger;
    private readonly ITelegramHandleService _telegramService;
    private readonly ISteamWebApiClient _webApiClient;

    /// <inheritdoc cref="BotController"/>
    public BotController(
        ILogger<BotController> logger,
        ITelegramHandleService telegramService, 
        ISteamWebApiClient webApiClient
        )
    {
        _logger = logger;
        _telegramService = telegramService;
        _webApiClient = webApiClient;
    }

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