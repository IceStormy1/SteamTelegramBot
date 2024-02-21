using Microsoft.AspNetCore.Mvc;
using SteamTelegramBot.Abstractions.Services;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Clients.Models;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
    private readonly ILogger<BotController> _logger;
    private readonly IStoreSteamApiClient _storeSteamApiClient;
    private readonly ITelegramService _telegramService;

    public BotController(
        ILogger<BotController> logger,
        IStoreSteamApiClient storeSteamApiClient, 
        ITelegramService telegramService
        )
    {
        _logger = logger;
        _storeSteamApiClient = storeSteamApiClient;
        _telegramService = telegramService;
    }

    [HttpGet("test")]
    public async Task<ResultData> Test()
    {
        var test = await _storeSteamApiClient.GetAppDetails(1228580);

        return test;
    }

    [HttpPost]
    public async Task<IActionResult> Update(
        //[FromBody] Update update,
        CancellationToken cancellationToken
    )
    {
        //await _telegramService.HandleUpdateAsync(update, cancellationToken);
        await _telegramService.HandleUpdateAsync(new Update(), cancellationToken);

        return Ok();
    }
}