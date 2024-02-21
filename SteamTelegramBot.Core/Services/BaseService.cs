using AutoMapper;
using Microsoft.Extensions.Logging;

namespace SteamTelegramBot.Core.Services;

public abstract class BaseService
{
    protected readonly IMapper Mapper;
    protected readonly ILogger<BaseService> Logger;

    protected BaseService(IMapper mapper, ILogger<BaseService> logger)
    {
        Mapper = mapper;
        Logger = logger;
    }
}