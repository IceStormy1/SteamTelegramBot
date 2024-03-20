using Newtonsoft.Json;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Callbacks;

/// <summary>
/// Represents a base class for handling callback queries in a Telegram bot.
/// </summary>
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

    /// <summary>
    /// Name of the callback.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Executes the callback action.
    /// </summary>
    /// <param name="callbackQuery">The callback query received from the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public abstract Task Execute(CallbackQuery callbackQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Deserializes callback data from a callback query.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the callback data to.</typeparam>
    /// <param name="callbackQuery">The callback query.</param>
    /// <returns>The deserialized callback data.</returns>
    public static T GetCallbackData<T>(CallbackQuery callbackQuery)
        => JsonConvert.DeserializeObject<T>(callbackQuery.Data ?? string.Empty);
}