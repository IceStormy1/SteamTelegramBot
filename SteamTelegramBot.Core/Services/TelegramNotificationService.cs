using AutoMapper;
using Microsoft.Extensions.Logging;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Common.Extensions;
using SteamTelegramBot.Core.Extensions;
using SteamTelegramBot.Core.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SteamTelegramBot.Core.Services;

internal sealed class TelegramNotificationService : BaseService, ITelegramNotificationService
{
    private readonly ITelegramBotClient _botClient;

    public TelegramNotificationService(
        IMapper mapper, 
        ILogger<BaseService> logger, 
        ITelegramBotClient botClient) : base(mapper, logger)
    {
        _botClient = botClient;
    }

    public async Task NotifyUsersOfPriceDrop(Dictionary<long, List<SteamSuggestItem>> usersWithDiscountedApps)
    {
        var tasks = usersWithDiscountedApps.Select(pair => NotifyUser(pair.Key, pair.Value));
        await Task.WhenAll(tasks);
    }

    private async Task NotifyUser(long telegramChatId, IEnumerable<SteamSuggestItem> discountedApps)
    {
        var formattedListOfDiscountedApps = discountedApps.Select((item, index) => item.ToTelegramHyperlink(index));

        var messageText = FormatDiscountedAppsMessage(formattedListOfDiscountedApps);

        await SendTelegramMessage(telegramChatId, messageText);
    }

    private static string FormatDiscountedAppsMessage(IEnumerable<string> formattedApps)
    {
        var appsList = string.Join("\n", formattedApps);
        return $"Из вашего списка снизилась цена на следующие товары:\n{appsList}";
    }

    private async Task SendTelegramMessage(long chatId, string text)
    {
        text = text.ToTelegramMarkdownMessageText();
        await _botClient.SendTextMessageAsync(chatId: chatId, text: text, parseMode: ParseMode.MarkdownV2, disableWebPagePreview: true);
    }
}