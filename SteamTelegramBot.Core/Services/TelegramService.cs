using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Services;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Services;

public sealed class TelegramService : BaseService, ITelegramService
{
    public TelegramService(
        IMapper mapper,
        ILogger<BaseService> logger
        ) : base(mapper, logger)
    {
    }

    public async Task Test()
    {
        var test = new Message();

    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        var test = new Message();
    }
}