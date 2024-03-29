using Microsoft.Extensions.Options;
using SteamTelegramBot.Common.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Configurations;

/// <summary>
/// Configuring webhook
/// </summary>
public class ConfigureWebhook : IHostedService
{
    private readonly ILogger<ConfigureWebhook> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotConfiguration _botConfig;

    /// <inheritdoc cref="ConfigureWebhook"/>
    public ConfigureWebhook(
        ILogger<ConfigureWebhook> logger,
        IServiceProvider serviceProvider,
        IOptions<BotConfiguration> botOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _botConfig = botOptions.Value;
    }

    /// <summary>
    /// Set webhook
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var webHookAddress = $"{_botConfig.HostAddress}{_botConfig.Route}";
        _logger.LogInformation("Setting webhook: {WebHookAddress}", webHookAddress);
    
        await botClient.SetWebhookAsync(
            url: webHookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            secretToken: _botConfig.SecretToken,
            cancellationToken: cancellationToken);

        await botClient.SetMyCommandsAsync(
            cancellationToken: cancellationToken,
            scope: BotCommandScope.AllPrivateChats(),
            commands: new List<BotCommand>
            {
                new() { Command = TelegramCommands.StartCommand, Description = TelegramBotMessages.RestartBot },
                new() { Command = TelegramCommands.AddGameCommand, Description = TelegramBotMessages.AddGameForTracking }
            });

        await botClient.SetMyDescriptionAsync(TelegramBotMessages.BotDescription, cancellationToken: cancellationToken);

        var shortDescription = string.Format(TelegramBotMessages.BotShortDescriptionFormat, _botConfig.OwnerUsername);
        await botClient.SetMyShortDescriptionAsync(shortDescription, cancellationToken: cancellationToken);
        
        _logger.LogInformation("Setting webhook was complete");
    }

    /// <summary>
    /// Remove webhook
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Removing webhook was complete");
    }
}