namespace SteamTelegramBot.Core.Interfaces;

public interface ITelegramNotificationService
{
    Task NotifyUsersOfPriceDrop();
}