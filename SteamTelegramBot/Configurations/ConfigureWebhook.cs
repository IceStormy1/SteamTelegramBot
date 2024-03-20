using Microsoft.Extensions.Options;
using SteamTelegramBot.Common.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Configurations;

public class ConfigureWebhook : IHostedService
{
    private readonly ILogger<ConfigureWebhook> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotConfiguration _botConfig;

    public ConfigureWebhook(
        ILogger<ConfigureWebhook> logger,
        IServiceProvider serviceProvider,
        IOptions<BotConfiguration> botOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _botConfig = botOptions.Value;
    }

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
                new() { Command = TelegramCommands.StartCommand, Description = "Перезапустить бота" },
                new() { Command = TelegramCommands.AddGameCommand, Description = "Добавить игру для отслеживания цены" }
            });

        await botClient.SetMyDescriptionAsync(TelegramMessages.BotDescription, cancellationToken: cancellationToken);
        await botClient.SetMyShortDescriptionAsync($"По всем вопросам обращаться к @{_botConfig.OwnerUsername}", cancellationToken: cancellationToken);
        
        _logger.LogInformation("Setting webhook was complete");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Removing webhook was complete");
    }
}