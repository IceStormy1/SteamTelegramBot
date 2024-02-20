using Microsoft.AspNetCore.Mvc;
using SteamTelegramBot.Clients;
using SteamTelegramBot.Clients.Models;

namespace SteamTelegramBot.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
    private readonly ILogger<BotController> _logger;
    private readonly IStoreSteamApiClient _storeSteamApiClient;

    public BotController(
        ILogger<BotController> logger,
        IStoreSteamApiClient storeSteamApiClient
        )
    {
        _logger = logger;
        _storeSteamApiClient = storeSteamApiClient;
    }

    [HttpGet]
    public async Task<ResultData> Get()
    {
        var test = await _storeSteamApiClient.GetAppDetails(1228580);

        return test;
    }
}