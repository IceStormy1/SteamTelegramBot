using Newtonsoft.Json;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

internal abstract class BaseCallback
{
    protected readonly ITelegramBotClient BotClient;
    protected readonly IUserAppTrackingService UserAppTrackingService;
    protected readonly ITelegramNotificationService TelegramNotificationService;

    protected BaseCallback(
        ITelegramBotClient botClient,
        IUserAppTrackingService userAppTrackingService, 
        ITelegramNotificationService telegramNotificationService)
    {
        BotClient = botClient;
        UserAppTrackingService = userAppTrackingService;
        TelegramNotificationService = telegramNotificationService;
    }

    public abstract string Name { get; }

    public static T GetCallbackData<T>(CallbackQuery callbackQuery)
        => JsonConvert.DeserializeObject<T>(callbackQuery.Data ?? string.Empty);

    public abstract Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken);
}