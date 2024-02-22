using Microsoft.AspNetCore.Mvc;
using SteamTelegramBot.Abstractions.Services;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Clients.Models;
using SteamTelegramBot.Filters;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
    private readonly ILogger<BotController> _logger;
    private readonly IStoreSteamApiClient _storeSteamApiClient;
    private readonly ITelegramService _telegramService;
    private readonly ISteamWebApiClient _webApiClient;

    public BotController(
        ILogger<BotController> logger,
        IStoreSteamApiClient storeSteamApiClient, 
        ITelegramService telegramService, 
        ISteamWebApiClient webApiClient
        )
    {
        _logger = logger;
        _storeSteamApiClient = storeSteamApiClient;
        _telegramService = telegramService;
        _webApiClient = webApiClient;
    }

    [HttpGet("test")]
    public async Task<List<AppItemDto>> Test()
    {
        var test = await _storeSteamApiClient.GetAppDetails(1228580);
        var test2 = (await _webApiClient.GetAllApps()).Content?.AppList?.Apps?.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();

        return test2;
    }

    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Update(
        [FromBody] Update update,
        CancellationToken cancellationToken
    )
    {
        //await _telegramService.HandleUpdateAsync(update, cancellationToken);
        await _telegramService.HandleUpdateAsync(update, cancellationToken);

        return Ok();
    }
}