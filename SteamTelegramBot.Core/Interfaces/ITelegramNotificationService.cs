using SteamTelegramBot.Abstractions.Models;

namespace SteamTelegramBot.Core.Interfaces;

public interface ITelegramNotificationService
{
    Task NotifyUsersOfPriceDrop(Dictionary<long, List<SteamSuggestItem>> usersWithDiscountedApps);
}